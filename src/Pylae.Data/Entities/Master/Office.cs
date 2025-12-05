namespace Pylae.Data.Entities.Master;

public class Office
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? HeadFullName { get; set; }
    public string? HeadBusinessTitle { get; set; }
    public string? HeadBusinessRank { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
