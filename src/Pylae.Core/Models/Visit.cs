using Pylae.Core.Enums;

namespace Pylae.Core.Models;

public class Visit
{
    public int Id { get; set; }
    public string? VisitGuid { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public int MemberNumber { get; set; }
    public string MemberFirstName { get; set; } = string.Empty;
    public string MemberLastName { get; set; } = string.Empty;
    public string? MemberBusinessRank { get; set; }
    public string? MemberOfficeName { get; set; }
    public bool MemberIsPermanentStaff { get; set; }
    public string? MemberTypeCode { get; set; }
    public string? MemberTypeName { get; set; }
    public string? MemberPersonalIdNumber { get; set; }
    public string? MemberBusinessIdNumber { get; set; }
    public VisitDirection Direction { get; set; }
    public DateTime TimestampUtc { get; set; }
    public DateTime TimestampLocal { get; set; }
    public VisitMethod Method { get; set; }
    public string SiteCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserDisplayName { get; set; }
    public string? WorkstationId { get; set; }
    public string? Notes { get; set; }
}
