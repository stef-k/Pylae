using Pylae.Core.Interfaces;

namespace Pylae.Desktop.Services;

/// <summary>
/// Singleton service that provides in-memory cached access to application settings.
/// Loaded during app warmup and refreshed when settings are saved.
/// </summary>
public class AppSettingsService : IAppSettings
{
    private readonly ISettingsService _settingsService;
    private Dictionary<string, string> _cache = new();
    private readonly object _lock = new();

    public AppSettingsService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public string GetValue(string key, string defaultValue = "")
    {
        lock (_lock)
        {
            return _cache.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var value = GetValue(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        var value = GetValue(key);
        return value == "1" || (defaultValue && string.IsNullOrEmpty(value));
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAllAsync(cancellationToken);
        lock (_lock)
        {
            _cache = new Dictionary<string, string>(settings);
        }
    }
}
