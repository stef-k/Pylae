namespace Pylae.Core.Interfaces;

/// <summary>
/// Provides synchronous access to cached application settings.
/// Settings are loaded once at app startup and refreshed when saved.
/// </summary>
public interface IAppSettings
{
    /// <summary>
    /// Gets a setting value by key.
    /// </summary>
    string GetValue(string key, string defaultValue = "");

    /// <summary>
    /// Gets a setting value as an integer.
    /// </summary>
    int GetInt(string key, int defaultValue = 0);

    /// <summary>
    /// Gets a setting value as a boolean (stored as "1" or "0").
    /// </summary>
    bool GetBool(string key, bool defaultValue = false);

    /// <summary>
    /// Loads or refreshes all settings from the database into memory.
    /// Called at app startup and after settings are saved.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);
}
