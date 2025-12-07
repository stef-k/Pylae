using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;
using Pylae.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Pylae.Desktop.Services;

/// <summary>
/// Background service that performs periodic cleanup of old visit records.
/// </summary>
public class VisitCleanupService : IDisposable
{
    private readonly PylaeVisitsDbContext _dbContext;
    private readonly IAppSettings _appSettings;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<VisitCleanupService>? _logger;
    private System.Threading.Timer? _timer;
    private bool _disposed;

    public VisitCleanupService(
        PylaeVisitsDbContext dbContext,
        IAppSettings appSettings,
        ISettingsService settingsService,
        ILogger<VisitCleanupService>? logger = null)
    {
        _dbContext = dbContext;
        _appSettings = appSettings;
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Starts the visit cleanup service.
    /// </summary>
    public Task StartAsync()
    {
        var years = _appSettings.GetInt(SettingKeys.VisitRetentionYears, 0);

        if (years <= 0)
        {
            _logger?.LogInformation("Visit retention is disabled (retention years: {Years})", years);
            return Task.CompletedTask;
        }

        // Check if cleanup is overdue (catch-up logic)
        var initialDelay = GetInitialDelay();

        // Run cleanup once per week (visits table can be large)
        _timer = new System.Threading.Timer(
            async _ => await PerformCleanupAsync(years),
            null,
            (int)initialDelay.TotalMilliseconds,
            7 * 24 * 60 * 60 * 1000); // Then every 7 days

        _logger?.LogInformation("Visit cleanup service started (retention: {Years} years, runs weekly, initial delay: {Delay})", years, initialDelay);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Calculates initial delay, executing immediately if cleanup is overdue.
    /// </summary>
    private TimeSpan GetInitialDelay()
    {
        var lastCleanupStr = _appSettings.GetValue(SettingKeys.LastVisitCleanupTime);
        if (!string.IsNullOrEmpty(lastCleanupStr) && DateTime.TryParse(lastCleanupStr, out var lastCleanup))
        {
            var nextCleanupDue = lastCleanup.AddDays(7);
            if (DateTime.UtcNow >= nextCleanupDue)
            {
                _logger?.LogInformation("Visit cleanup is overdue (last: {LastCleanup}, due: {DueTime}), executing immediately",
                    lastCleanup, nextCleanupDue);
                return TimeSpan.Zero; // Execute immediately
            }

            var delay = nextCleanupDue - DateTime.UtcNow;
            _logger?.LogInformation("Next visit cleanup scheduled for {NextCleanup}", nextCleanupDue);
            return delay;
        }

        // No previous cleanup recorded, run after 1 minute
        return TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Stops the visit cleanup service.
    /// </summary>
    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger?.LogInformation("Visit cleanup service stopped");
    }

    private async Task PerformCleanupAsync(int retentionYears)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddYears(-retentionYears);

            _logger?.LogInformation("Starting visit cleanup (removing visits before {CutoffDate})", cutoffDate);

            var deleted = await _dbContext.Visits
                .Where(v => v.TimestampUtc < cutoffDate)
                .ExecuteDeleteAsync();

            // Save last cleanup time
            await _settingsService.UpsertAsync(new[]
            {
                new Core.Models.Setting { Key = SettingKeys.LastVisitCleanupTime, Value = DateTime.UtcNow.ToString("O") }
            });

            if (deleted > 0)
            {
                _logger?.LogInformation("Visit cleanup completed: {Count} visits removed", deleted);
            }
            else
            {
                _logger?.LogDebug("Visit cleanup completed: no visits to remove");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Visit cleanup failed");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _timer?.Dispose();
        _disposed = true;
    }
}
