using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;
using Pylae.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Pylae.Desktop.Services;

/// <summary>
/// Background service that performs periodic cleanup of old audit log entries.
/// </summary>
public class AuditLogCleanupService : IDisposable
{
    private readonly PylaeMasterDbContext _dbContext;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AuditLogCleanupService>? _logger;
    private System.Threading.Timer? _timer;
    private bool _disposed;

    public AuditLogCleanupService(
        PylaeMasterDbContext dbContext,
        ISettingsService settingsService,
        ILogger<AuditLogCleanupService>? logger = null)
    {
        _dbContext = dbContext;
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Starts the audit log cleanup service.
    /// </summary>
    public async Task StartAsync()
    {
        var settings = await _settingsService.GetAllAsync();

        if (!settings.TryGetValue(SettingKeys.AuditRetentionYears, out var yearsStr) ||
            !int.TryParse(yearsStr, out var years) ||
            years <= 0)
        {
            _logger?.LogInformation("Audit log retention is disabled (retention years: {Years})", yearsStr ?? "0");
            return;
        }

        // Check if cleanup is overdue (catch-up logic)
        var initialDelay = await GetInitialDelayAsync(settings);

        // Run cleanup once per day
        _timer = new System.Threading.Timer(
            async _ => await PerformCleanupAsync(years),
            null,
            (int)initialDelay.TotalMilliseconds,
            24 * 60 * 60 * 1000); // Then every 24 hours

        _logger?.LogInformation("Audit log cleanup service started (retention: {Years} years, initial delay: {Delay})", years, initialDelay);
    }

    /// <summary>
    /// Calculates initial delay, executing immediately if cleanup is overdue.
    /// </summary>
    private async Task<TimeSpan> GetInitialDelayAsync(IDictionary<string, string> settings)
    {
        if (settings.TryGetValue(SettingKeys.LastAuditCleanupTime, out var lastCleanupStr) &&
            DateTime.TryParse(lastCleanupStr, out var lastCleanup))
        {
            var nextCleanupDue = lastCleanup.AddHours(24);
            if (DateTime.UtcNow >= nextCleanupDue)
            {
                _logger?.LogInformation("Audit cleanup is overdue (last: {LastCleanup}, due: {DueTime}), executing immediately",
                    lastCleanup, nextCleanupDue);
                return TimeSpan.Zero; // Execute immediately
            }

            var delay = nextCleanupDue - DateTime.UtcNow;
            _logger?.LogInformation("Next audit cleanup scheduled for {NextCleanup}", nextCleanupDue);
            return delay;
        }

        // No previous cleanup recorded, run after 1 minute
        return TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Stops the audit log cleanup service.
    /// </summary>
    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger?.LogInformation("Audit log cleanup service stopped");
    }

    private async Task PerformCleanupAsync(int retentionYears)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddYears(-retentionYears);

            _logger?.LogInformation("Starting audit log cleanup (removing entries before {CutoffDate})", cutoffDate);

            var deleted = await _dbContext.AuditEntries
                .Where(a => a.TimestampUtc < cutoffDate)
                .ExecuteDeleteAsync();

            // Save last cleanup time
            await _settingsService.UpsertAsync(new[]
            {
                new Core.Models.Setting { Key = SettingKeys.LastAuditCleanupTime, Value = DateTime.UtcNow.ToString("O") }
            });

            if (deleted > 0)
            {
                _logger?.LogInformation("Audit log cleanup completed: {Count} entries removed", deleted);
            }
            else
            {
                _logger?.LogDebug("Audit log cleanup completed: no entries to remove");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Audit log cleanup failed");
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
