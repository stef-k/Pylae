namespace Pylae.Core.Models;

public class BadgeStatus
{
    public BadgeStatus(DateTime? issueDate, DateTime? expiryDate, bool isExpired, bool isWarning, string? message)
    {
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        IsExpired = isExpired;
        IsWarning = isWarning;
        Message = message;
    }

    public DateTime? IssueDate { get; }

    public DateTime? ExpiryDate { get; }

    public bool IsExpired { get; }

    public bool IsWarning { get; }

    public string? Message { get; }

    public DateTime? EffectiveExpiryDate => ExpiryDate;
}
