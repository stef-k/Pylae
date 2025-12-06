using System.Text.Json;
using Pylae.Sync.Models;

namespace Pylae.Sync.Client;

/// <summary>
/// Manages remote site configurations stored per-machine (not per-user or in database).
/// Configuration is stored in C:\ProgramData\Pylae\{siteCode}\Config\remotesites.json
/// </summary>
public class MachineRemoteSitesConfig
{
    private readonly string _configPath;
    private List<RemoteSiteConfig> _sites = new();

    public MachineRemoteSitesConfig(string siteCode)
    {
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Pylae",
            siteCode,
            "Config");

        Directory.CreateDirectory(basePath);
        _configPath = Path.Combine(basePath, "remotesites.json");
    }

    /// <summary>
    /// Gets the list of configured remote sites.
    /// </summary>
    public IReadOnlyList<RemoteSiteConfig> Sites => _sites.AsReadOnly();

    /// <summary>
    /// Loads the remote sites configuration from disk.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_configPath))
        {
            _sites = new List<RemoteSiteConfig>();
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_configPath, cancellationToken);
            _sites = JsonSerializer.Deserialize<List<RemoteSiteConfig>>(json) ?? new List<RemoteSiteConfig>();
        }
        catch
        {
            _sites = new List<RemoteSiteConfig>();
        }
    }

    /// <summary>
    /// Saves the remote sites configuration to disk.
    /// </summary>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(_sites, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_configPath, json, cancellationToken);
    }

    /// <summary>
    /// Adds or updates a remote site configuration.
    /// </summary>
    public void AddOrUpdateSite(RemoteSiteConfig site)
    {
        var existing = _sites.FirstOrDefault(s => s.SiteCode == site.SiteCode);
        if (existing != null)
        {
            existing.Host = site.Host;
            existing.Port = site.Port;
            existing.ApiKey = site.ApiKey;
            existing.DisplayName = site.DisplayName;
        }
        else
        {
            _sites.Add(site);
        }
    }

    /// <summary>
    /// Removes a remote site configuration by site code.
    /// </summary>
    public bool RemoveSite(string siteCode)
    {
        var site = _sites.FirstOrDefault(s => s.SiteCode == siteCode);
        if (site != null)
        {
            _sites.Remove(site);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a remote site configuration by site code.
    /// </summary>
    public RemoteSiteConfig? GetSite(string siteCode)
    {
        return _sites.FirstOrDefault(s => s.SiteCode == siteCode);
    }

    /// <summary>
    /// Gets the list of site display names for user-friendly selection (doesn't expose IPs/keys).
    /// </summary>
    public IReadOnlyList<(string SiteCode, string DisplayName)> GetSiteList()
    {
        return _sites.Select(s => (s.SiteCode, s.DisplayName ?? s.SiteCode)).ToList();
    }
}
