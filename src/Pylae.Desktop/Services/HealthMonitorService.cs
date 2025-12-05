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
    private readonly ISettingsService _settingsService;
    private readonly ILogger<HealthMonitorService> _logger;
    private readonly System.Threading.Timer _timer;
    private readonly int _intervalMinutes = 5;
    private bool _enabled;

    public HealthMonitorService(ISettingsService settingsService, ILogger<HealthMonitorService> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
        _timer = new System.Threading.Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    public async Task StartAsync()
    {
        var settings = await _settingsService.GetAllAsync();
        _enabled = settings.TryGetValue(SettingKeys.HealthLoggingEnabled, out var flag) && flag == "1";
        if (_enabled)
        {
            _timer.Change(TimeSpan.FromMinutes(_intervalMinutes), TimeSpan.FromMinutes(_intervalMinutes));
            _logger.LogInformation("Health monitor started. Interval: {Interval} minutes", _intervalMinutes);
        }
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
