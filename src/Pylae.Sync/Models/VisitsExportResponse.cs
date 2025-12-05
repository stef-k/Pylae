namespace Pylae.Sync.Models;

public class VisitsExportResponse
{
    public string ContentType { get; set; } = "application/json";

    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
