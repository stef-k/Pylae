namespace Pylae.Core.Models;

public class Member
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int MemberNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? BusinessRank { get; set; }
    public int? OfficeId { get; set; }
    public Office? Office { get; set; }
    public bool IsPermanentStaff { get; set; }
    public int? MemberTypeId { get; set; }
    public MemberType? MemberType { get; set; }
    public string? PersonalIdNumber { get; set; }
    public string? BusinessIdNumber { get; set; }
    public string? PhotoFileName { get; set; }
    public DateTime? BadgeIssueDate { get; set; }
    public DateTime? BadgeExpiryDate { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
