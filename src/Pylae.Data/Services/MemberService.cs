using Microsoft.EntityFrameworkCore;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using MemberEntity = Pylae.Data.Entities.Master.Member;
using MemberTypeEntity = Pylae.Data.Entities.Master.MemberType;

namespace Pylae.Data.Services;

public class MemberService : IMemberService
{
    private readonly PylaeMasterDbContext _dbContext;
    private readonly IClock _clock;

    public MemberService(PylaeMasterDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Member?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Members
            .AsNoTracking()
            .Include(m => m.MemberType)
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Member?> GetActiveByMemberNumberAsync(int memberNumber, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Members
            .AsNoTracking()
            .Include(m => m.MemberType)
            .Where(m => m.MemberNumber == memberNumber && m.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<Member>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var normalized = query?.Trim();
        var members = _dbContext.Members
            .AsNoTracking()
            .Include(m => m.MemberType)
            .Where(m => m.IsActive);

        if (!string.IsNullOrWhiteSpace(normalized))
        {
            members = members.Where(m =>
                m.FirstName.Contains(normalized) ||
                m.LastName.Contains(normalized) ||
                m.MemberNumber.ToString().Contains(normalized));
        }

        var list = await members
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync(cancellationToken);

        return list.Select(ToDomain).ToList();
    }

    public async Task<int> GetNextAvailableMemberNumberAsync(CancellationToken cancellationToken = default)
    {
        // Get all member numbers currently in use by active members
        var usedNumbers = await _dbContext.Members
            .Where(m => m.IsActive)
            .Select(m => m.MemberNumber)
            .OrderBy(n => n)
            .ToListAsync(cancellationToken);

        // Find the lowest available number starting from 1
        var nextNumber = 1;
        foreach (var usedNumber in usedNumbers)
        {
            if (usedNumber > nextNumber)
            {
                // Found a gap - use this number
                break;
            }
            if (usedNumber == nextNumber)
            {
                nextNumber++;
            }
        }

        return nextNumber;
    }

    public async Task<Member> CreateAsync(Member member, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(member);

        // Always auto-assign the next available member number
        entity.MemberNumber = await GetNextAvailableMemberNumberAsync(cancellationToken);
        entity.CreatedAtUtc = _clock.UtcNow;
        entity.IsActive = true;

        _dbContext.Members.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDomain(entity);
    }

    public async Task<Member> UpdateAsync(Member member, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == member.Id, cancellationToken);
        if (existing is null)
        {
            throw new InvalidOperationException("Member not found.");
        }

        // Preserve original MemberNumber (immutable after creation)
        var originalMemberNumber = existing.MemberNumber;
        CopyMember(member, existing);
        existing.MemberNumber = originalMemberNumber;
        existing.UpdatedAtUtc = _clock.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(existing);
    }

    public async Task DeleteAsync(string memberId, CancellationToken cancellationToken = default)
    {
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
        if (member is null)
        {
            return;
        }

        _dbContext.Members.Remove(member);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkInactiveAsync(string memberId, CancellationToken cancellationToken = default)
    {
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
        if (member is null)
        {
            return;
        }

        member.IsActive = false;
        member.UpdatedAtUtc = _clock.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Member> IssueBadgeAsync(string memberId, DateTime issueDate, int badgeValidityMonths, CancellationToken cancellationToken = default)
    {
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
        if (member is null)
        {
            throw new InvalidOperationException("Member not found.");
        }

        member.BadgeIssueDate = issueDate.Date;
        if (badgeValidityMonths > 0)
        {
            member.BadgeExpiryDate = issueDate.Date.AddMonths(badgeValidityMonths);
        }

        member.UpdatedAtUtc = _clock.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(member);
    }

    private static MemberEntity MapToEntity(Member member)
    {
        var entity = new MemberEntity();
        CopyMember(member, entity);
        return entity;
    }

    private static void CopyMember(Member source, MemberEntity target)
    {
        target.Id = source.Id;
        target.MemberNumber = source.MemberNumber;
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
        target.BusinessRank = source.BusinessRank;
        target.Office = source.Office;
        target.IsPermanentStaff = source.IsPermanentStaff;
        target.MemberTypeId = source.MemberTypeId;
        target.PersonalIdNumber = source.PersonalIdNumber;
        target.BusinessIdNumber = source.BusinessIdNumber;
        target.PhotoFileName = source.PhotoFileName;
        target.BadgeIssueDate = source.BadgeIssueDate;
        target.BadgeExpiryDate = source.BadgeExpiryDate;
        target.DateOfBirth = source.DateOfBirth;
        target.Phone = source.Phone;
        target.Email = source.Email;
        target.Notes = source.Notes;
        target.IsActive = source.IsActive;
        target.CreatedAtUtc = source.CreatedAtUtc;
        target.UpdatedAtUtc = source.UpdatedAtUtc;
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
