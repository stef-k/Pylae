using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Data.Context;
using Serilog;
using Serilog.Events;

namespace Pylae.Desktop.Services;

public static class LoggingConfigurator
{
    public static ILoggerFactory ConfigureSerilog(DatabaseOptions dbOptions, IReadOnlyDictionary<string, string> settings)
    {
        Directory.CreateDirectory(dbOptions.GetLogsPath());
        var logPath = Path.Combine(dbOptions.GetLogsPath(), "pylae.log");
        var retentionDays = ParseInt(settings, SettingKeys.LogRetentionDays, 30);
        var level = ParseLogLevel(settings.TryGetValue(SettingKeys.LogLevel, out var levelValue) ? levelValue : "Information");
        var maxSizeMb = ParseInt(settings, SettingKeys.LogFileMaxSizeMB, 10);
        var fileSizeLimit = maxSizeMb > 0 ? maxSizeMb * 1024L * 1024L : (long?)null;
        CleanOldLogs(dbOptions.GetLogsPath(), retentionDays);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(level)
            .WriteTo.File(
                logPath,
                fileSizeLimitBytes: fileSizeLimit,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retentionDays > 0 ? retentionDays : null)
            .CreateLogger();

        return LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        });
    }

    private static int ParseInt(IReadOnlyDictionary<string, string> settings, string key, int defaultValue)
    {
        if (settings.TryGetValue(key, out var value) && int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        return defaultValue;
    }

    private static LogEventLevel ParseLogLevel(string? level)
    {
        if (Enum.TryParse<LogEventLevel>(level, true, out var parsed))
        {
            return parsed;
        }

        return LogEventLevel.Information;
    }

    private static void CleanOldLogs(string logsPath, int retentionDays)
    {
        if (retentionDays <= 0 || !Directory.Exists(logsPath))
        {
            return;
        }

        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        foreach (var file in Directory.GetFiles(logsPath, "*.log", SearchOption.TopDirectoryOnly))
        {
            var info = new FileInfo(file);
            if (info.LastWriteTimeUtc < cutoff)
            {
                try
                {
                    info.Delete();
                }
                catch
                {
                    // ignore cleanup failures
                }
            }
        }
    }
}
