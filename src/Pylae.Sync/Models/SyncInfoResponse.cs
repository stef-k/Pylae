namespace Pylae.Sync.Models;

public class SyncInfoResponse
{
    public string SiteCode { get; set; } = string.Empty;

    public string? SiteDisplayName { get; set; }

    public int MemberCount { get; set; }

    public long VisitsCount { get; set; }

    public DateTime? LastVisitTimestampUtc { get; set; }
}
