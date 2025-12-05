namespace Pylae.Core.Models;

public class VisitLogResult
{
    public VisitLogResult(Visit visit, Member member, BadgeStatus badgeStatus)
    {
        Visit = visit;
        Member = member;
        BadgeStatus = badgeStatus;
    }

    public Visit Visit { get; }

    public Member Member { get; }

    public BadgeStatus BadgeStatus { get; }
}
