namespace Pylae.Sync.Models;

public class MasterPackage
{
    public string SiteCode { get; set; } = string.Empty;

    public byte[] MasterDatabase { get; set; } = Array.Empty<byte>();

    public byte[]? PhotosArchive { get; set; }
}
