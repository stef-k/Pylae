using Microsoft.Extensions.Logging;
using System.Threading;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;

namespace Pylae.Desktop.Services;

/// <summary>
/// Periodic health logger controlled by HealthLoggingEnabled setting.
/// </summary>
public class HealthMonitorService : IDisposable
{
    private readonly IAppSettings _appSettings;
    private readonly ILogger<HealthMonitorService> _logger;
    private readonly System.Threading.Timer _timer;
    private readonly int _intervalMinutes = 5;
    private bool _enabled;

    public HealthMonitorService(IAppSettings appSettings, ILogger<HealthMonitorService> logger)
    {
        _appSettings = appSettings;
        _logger = logger;
        _timer = new System.Threading.Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    public Task StartAsync()
    {
        _enabled = _appSettings.GetBool(SettingKeys.HealthLoggingEnabled);
        if (_enabled)
        {
            _timer.Change(TimeSpan.FromMinutes(_intervalMinutes), TimeSpan.FromMinutes(_intervalMinutes));
            _logger.LogInformation("Health monitor started. Interval: {Interval} minutes", _intervalMinutes);
        }
        return Task.CompletedTask;
    }

    private void OnTick(object? _)
    {
        if (!_enabled)
        {
            return;
        }

        _logger.LogInformation("Health: alive at {TimestampUtc}", DateTime.UtcNow);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
