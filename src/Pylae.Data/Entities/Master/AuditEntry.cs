namespace Pylae.Data.Entities.Master;

public class AuditEntry
{
    public int Id { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string SiteCode { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? TargetId { get; set; }
    public string? DetailsJson { get; set; }
}
