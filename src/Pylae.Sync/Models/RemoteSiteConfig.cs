namespace Pylae.Sync.Models;

public class RemoteSiteConfig
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 8080;

    public string ApiKey { get; set; } = string.Empty;

    public string SiteCode { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public Uri ToBaseUri() => new Uri($"http://{Host}:{Port}");
}
