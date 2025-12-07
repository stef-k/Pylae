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
    private readonly IAppSettings _appSettings;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ScheduledBackupService>? _logger;
    private System.Threading.Timer? _timer;
    private bool _disposed;

    public ScheduledBackupService(
        IBackupService backupService,
        IAppSettings appSettings,
        ISettingsService settingsService,
        ILogger<ScheduledBackupService>? logger = null)
    {
        _backupService = backupService;
        _appSettings = appSettings;
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Starts the scheduled backup service.
    /// </summary>
    public Task StartAsync()
    {
        if (!_appSettings.GetBool(SettingKeys.AutoBackupEnabled))
        {
            _logger?.LogInformation("Automated backups are disabled");
            return Task.CompletedTask;
        }

        var intervalHours = _appSettings.GetInt(SettingKeys.AutoBackupIntervalHours, 24);
        if (intervalHours <= 0) intervalHours = 24;

        var retentionCount = _appSettings.GetInt(SettingKeys.AutoBackupRetentionCount, 7);
        if (retentionCount <= 0) retentionCount = 7;

        // Check if backup is overdue (catch-up logic)
        var initialDelay = GetInitialDelay(intervalHours);

        var intervalMs = intervalHours * 60 * 60 * 1000;
        _timer = new System.Threading.Timer(
            async _ => await PerformBackupAsync(retentionCount),
            null,
            (int)initialDelay.TotalMilliseconds,
            intervalMs);

        _logger?.LogInformation("Scheduled backup service started (interval: {Hours} hours, retention: {Count}, initial delay: {Delay})",
            intervalHours, retentionCount, initialDelay);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Calculates initial delay, executing immediately if backup is overdue.
    /// </summary>
    private TimeSpan GetInitialDelay(int intervalHours)
    {
        var lastBackupStr = _appSettings.GetValue(SettingKeys.LastBackupTime);
        if (!string.IsNullOrEmpty(lastBackupStr) && DateTime.TryParse(lastBackupStr, out var lastBackup))
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
            var retentionCount = _appSettings.GetInt(SettingKeys.AutoBackupRetentionCount, 7);
            if (retentionCount <= 0) retentionCount = 7;

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

            var backupPath = _appSettings.GetValue(SettingKeys.AutoBackupPath);
            if (string.IsNullOrWhiteSpace(backupPath))
            {
                backupPath = GetDefaultBackupPath();
            }

            Directory.CreateDirectory(backupPath);

            var includePhotos = _appSettings.GetBool(SettingKeys.AutoBackupIncludePhotos, true);

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

    private static string GetDefaultBackupPath()
    {
        // Check for portable mode marker file
        var portableMarkerPath = Path.Combine(AppContext.BaseDirectory, "portable.mode");
        if (File.Exists(portableMarkerPath))
        {
            return Path.Combine(AppContext.BaseDirectory, "PylaeData", "Backups");
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pylae", "Backups");
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
