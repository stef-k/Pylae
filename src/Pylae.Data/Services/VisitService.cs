using Microsoft.EntityFrameworkCore;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using VisitEntity = Pylae.Data.Entities.Visits.Visit;
using MemberEntity = Pylae.Data.Entities.Master.Member;
using MemberTypeEntity = Pylae.Data.Entities.Master.MemberType;

namespace Pylae.Data.Services;

public class VisitService : IVisitService
{
    private readonly PylaeMasterDbContext _masterDb;
    private readonly PylaeVisitsDbContext _visitsDb;
    private readonly IBadgeEvaluator _badgeEvaluator;
    private readonly IClock _clock;
    private readonly IAuditService _auditService;

    public VisitService(
        PylaeMasterDbContext masterDb,
        PylaeVisitsDbContext visitsDb,
        IBadgeEvaluator badgeEvaluator,
        IClock clock,
        IAuditService auditService)
    {
        _masterDb = masterDb;
        _visitsDb = visitsDb;
        _badgeEvaluator = badgeEvaluator;
        _clock = clock;
        _auditService = auditService;
    }

    public async Task<VisitLogResult> LogVisitAsync(
        int memberNumber,
        VisitDirection direction,
        VisitMethod method,
        string? notes,
        User actor,
        string siteCode,
        string? workstationId,
        int badgeValidityMonths,
        int badgeExpiryWarningDays,
        CancellationToken cancellationToken = default)
    {
        var member = await _masterDb.Members
            .Include(m => m.MemberType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber && m.IsActive, cancellationToken);

        if (member is null)
        {
            throw new InvalidOperationException("Member not found or inactive.");
        }

        var badgeStatus = _badgeEvaluator.Evaluate(
            member.BadgeIssueDate,
            member.BadgeExpiryDate,
            badgeValidityMonths,
            badgeExpiryWarningDays,
            _clock.Now);

        var timestampUtc = _clock.UtcNow;
        var visit = new VisitEntity
        {
            VisitGuid = Guid.NewGuid().ToString(),
            MemberId = member.Id,
            MemberNumber = member.MemberNumber,
            MemberFirstName = member.FirstName,
            MemberLastName = member.LastName,
            MemberBusinessRank = member.BusinessRank,
            MemberOfficeName = member.Office,
            MemberIsPermanentStaff = member.IsPermanentStaff,
            MemberTypeCode = member.MemberType?.Code,
            MemberTypeName = member.MemberType?.DisplayName,
            MemberPersonalIdNumber = member.PersonalIdNumber,
            MemberBusinessIdNumber = member.BusinessIdNumber,
            Direction = direction,
            TimestampUtc = timestampUtc,
            TimestampLocal = _clock.Now,
            Method = method,
            SiteCode = siteCode,
            UserId = actor.Id,
            Username = actor.Username,
            UserDisplayName = $"{actor.FirstName} {actor.LastName}",
            WorkstationId = workstationId,
            Notes = notes
        };

        _visitsDb.Visits.Add(visit);
        await _visitsDb.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(new AuditEntry
        {
            TimestampUtc = timestampUtc,
            SiteCode = siteCode,
            UserId = actor.Id,
            Username = actor.Username,
            ActionType = badgeStatus.IsExpired ? AuditActionTypes.UseExpiredBadge : AuditActionTypes.VisitLogged,
            TargetType = "Visit",
            TargetId = visit.VisitGuid,
            DetailsJson = notes
        }, cancellationToken);

        return new VisitLogResult(ToDomain(visit), ToDomain(member), badgeStatus);
    }

    public async Task<IReadOnlyCollection<Visit>> GetVisitsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var query = _visitsDb.Visits.AsNoTracking().AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(v => v.TimestampUtc >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(v => v.TimestampUtc <= to.Value);
        }

        var visits = await query
            .OrderByDescending(v => v.TimestampUtc)
            .ToListAsync(cancellationToken);

        return visits.Select(ToDomain).ToList();
    }

    public async Task UpdateVisitNotesAsync(int visitId, string? notes, int userId, CancellationToken cancellationToken = default)
    {
        var visit = await _visitsDb.Visits.FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);
        if (visit is null)
        {
            return;
        }

        visit.Notes = notes;
        await _visitsDb.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(new AuditEntry
        {
            TimestampUtc = _clock.UtcNow,
            SiteCode = visit.SiteCode,
            UserId = userId,
            Username = visit.Username,
            ActionType = AuditActionTypes.VisitNoteUpdated,
            TargetType = "Visit",
            TargetId = visit.VisitGuid,
            DetailsJson = notes
        }, cancellationToken);
    }

    private static Visit ToDomain(VisitEntity entity)
    {
        return new Visit
        {
            Id = entity.Id,
            VisitGuid = entity.VisitGuid,
            MemberId = entity.MemberId,
            MemberNumber = entity.MemberNumber,
            MemberFirstName = entity.MemberFirstName,
            MemberLastName = entity.MemberLastName,
            MemberBusinessRank = entity.MemberBusinessRank,
            MemberOfficeName = entity.MemberOfficeName,
            MemberIsPermanentStaff = entity.MemberIsPermanentStaff,
            MemberTypeCode = entity.MemberTypeCode,
            MemberTypeName = entity.MemberTypeName,
            MemberPersonalIdNumber = entity.MemberPersonalIdNumber,
            MemberBusinessIdNumber = entity.MemberBusinessIdNumber,
            Direction = entity.Direction,
            TimestampUtc = entity.TimestampUtc,
            TimestampLocal = entity.TimestampLocal,
            Method = entity.Method,
            SiteCode = entity.SiteCode,
            UserId = entity.UserId,
            Username = entity.Username,
            UserDisplayName = entity.UserDisplayName,
            WorkstationId = entity.WorkstationId,
            Notes = entity.Notes
        };
    }

    private static Member ToDomain(MemberEntity entity)
    {
        return new Member
        {
            Id = entity.Id,
            MemberNumber = entity.MemberNumber,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BusinessRank = entity.BusinessRank,
            Office = entity.Office,
            IsPermanentStaff = entity.IsPermanentStaff,
            MemberTypeId = entity.MemberTypeId,
            MemberType = entity.MemberType is null ? null : ToDomain(entity.MemberType),
            PersonalIdNumber = entity.PersonalIdNumber,
            BusinessIdNumber = entity.BusinessIdNumber,
            PhotoFileName = entity.PhotoFileName,
            BadgeIssueDate = entity.BadgeIssueDate,
            BadgeExpiryDate = entity.BadgeExpiryDate,
            DateOfBirth = entity.DateOfBirth,
            Phone = entity.Phone,
            Email = entity.Email,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }

    private static MemberType ToDomain(MemberTypeEntity entity)
    {
        return new MemberType
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
