using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;

namespace Pylae.Desktop.Services;

/// <summary>
/// Background service that performs automated scheduled backups.
/// </summary>
public class ScheduledBackupService : IDisposable
{
    private readonly IBackupService _backupService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ScheduledBackupService>? _logger;
    private System.Threading.Timer? _timer;
    private bool _disposed;

    public ScheduledBackupService(
        IBackupService backupService,
        ISettingsService settingsService,
        ILogger<ScheduledBackupService>? logger = null)
    {
        _backupService = backupService;
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Starts the scheduled backup service.
    /// </summary>
    public async Task StartAsync()
    {
        var settings = await _settingsService.GetAllAsync();

        if (!settings.TryGetValue(SettingKeys.AutoBackupEnabled, out var enabledStr) ||
            !bool.TryParse(enabledStr, out var enabled) ||
            !enabled)
        {
            _logger?.LogInformation("Automated backups are disabled");
            return;
        }

        var intervalHours = 24; // Default: daily
        if (settings.TryGetValue(SettingKeys.AutoBackupIntervalHours, out var intervalStr) &&
            int.TryParse(intervalStr, out var parsed) &&
            parsed > 0)
        {
            intervalHours = parsed;
        }

        var retentionCount = 7; // Default: keep last 7 backups
        if (settings.TryGetValue(SettingKeys.AutoBackupRetentionCount, out var retentionStr) &&
            int.TryParse(retentionStr, out var retentionParsed) &&
            retentionParsed > 0)
        {
            retentionCount = retentionParsed;
        }

        // Check if backup is overdue (catch-up logic)
        var initialDelay = await GetInitialDelayAsync(settings, intervalHours);

        var intervalMs = intervalHours * 60 * 60 * 1000;
        _timer = new System.Threading.Timer(
            async _ => await PerformBackupAsync(retentionCount),
            null,
            (int)initialDelay.TotalMilliseconds,
            intervalMs);

        _logger?.LogInformation("Scheduled backup service started (interval: {Hours} hours, retention: {Count}, initial delay: {Delay})",
            intervalHours, retentionCount, initialDelay);
    }

    /// <summary>
    /// Calculates initial delay, executing immediately if backup is overdue.
    /// </summary>
    private async Task<TimeSpan> GetInitialDelayAsync(IDictionary<string, string> settings, int intervalHours)
    {
        if (settings.TryGetValue(SettingKeys.LastBackupTime, out var lastBackupStr) &&
            DateTime.TryParse(lastBackupStr, out var lastBackup))
        {
            var nextBackupDue = lastBackup.AddHours(intervalHours);
            if (DateTime.UtcNow >= nextBackupDue)
            {
                _logger?.LogInformation("Backup is overdue (last: {LastBackup}, due: {DueTime}), executing immediately",
                    lastBackup, nextBackupDue);
                return TimeSpan.Zero; // Execute immediately
            }

            var delay = nextBackupDue - DateTime.UtcNow;
            _logger?.LogInformation("Next backup scheduled for {NextBackup}", nextBackupDue);
            return delay;
        }

        // No previous backup recorded, run after 1 minute
        return TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Stops the scheduled backup service.
    /// </summary>
    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger?.LogInformation("Scheduled backup service stopped");
    }

    /// <summary>
    /// Stops the scheduled backup service and performs a final shutdown backup.
    /// </summary>
    public async Task StopAsync()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger?.LogInformation("Scheduled backup service stopping, performing shutdown backup");

        try
        {
            var settings = await _settingsService.GetAllAsync();
            var retentionCount = 7;
            if (settings.TryGetValue(SettingKeys.AutoBackupRetentionCount, out var retentionStr) &&
                int.TryParse(retentionStr, out var parsed) &&
                parsed > 0)
            {
                retentionCount = parsed;
            }

            await PerformBackupAsync(retentionCount, isShutdownBackup: true);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Shutdown backup failed");
        }
    }

    private async Task PerformBackupAsync(int retentionCount, bool isShutdownBackup = false)
    {
        try
        {
            var backupType = isShutdownBackup ? "shutdown" : "automated";
            _logger?.LogInformation("Starting {BackupType} backup", backupType);

            var settings = await _settingsService.GetAllAsync();
            var backupPath = settings.TryGetValue(SettingKeys.AutoBackupPath, out var path) && !string.IsNullOrWhiteSpace(path)
                ? path
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pylae", "Backups");

            Directory.CreateDirectory(backupPath);

            var includePhotos = true;
            if (settings.TryGetValue(SettingKeys.AutoBackupIncludePhotos, out var photosStr) &&
                bool.TryParse(photosStr, out var photosParsed))
            {
                includePhotos = photosParsed;
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var prefix = isShutdownBackup ? "pylae_shutdown_backup" : "pylae_auto_backup";
            var backupFilePath = Path.Combine(backupPath, $"{prefix}_{timestamp}.zip");

            await _backupService.CreateBackupAsync(backupFilePath, includePhotos);

            // Save last backup time
            await _settingsService.UpsertAsync(new[]
            {
                new Core.Models.Setting { Key = SettingKeys.LastBackupTime, Value = DateTime.UtcNow.ToString("O") }
            });

            _logger?.LogInformation("{BackupType} backup completed: {Path}", backupType, backupFilePath);

            CleanOldBackups(backupPath, retentionCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Automated backup failed");
        }
    }

    private void CleanOldBackups(string backupPath, int retentionCount)
    {
        try
        {
            // Include both auto and shutdown backups in retention policy
            var backupFiles = Directory.GetFiles(backupPath, "pylae_*_backup_*.zip")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTimeUtc)
                .ToList();

            if (backupFiles.Count <= retentionCount)
            {
                return;
            }

            var filesToDelete = backupFiles.Skip(retentionCount);
            foreach (var file in filesToDelete)
            {
                try
                {
                    file.Delete();
                    _logger?.LogInformation("Deleted old backup: {Path}", file.FullName);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to delete old backup: {Path}", file.FullName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to clean old backups");
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
