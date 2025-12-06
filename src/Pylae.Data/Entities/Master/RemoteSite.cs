namespace Pylae.Data.Entities.Master;

public class RemoteSite
{
    public int Id { get; set; }
    public string SiteCode { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 8080;
    public string ApiKey { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
