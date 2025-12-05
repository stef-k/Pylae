namespace Pylae.Sync.Hosting;

public class SyncServerOptions
{
    public bool NetworkEnabled { get; set; }

    public int Port { get; set; } = 8080;

    public string ApiKey { get; set; } = string.Empty;

    public string SiteCode { get; set; } = string.Empty;

    public string? SiteDisplayName { get; set; }
}
