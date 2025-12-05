using Microsoft.EntityFrameworkCore;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using AuditEntryEntity = Pylae.Data.Entities.Master.AuditEntry;

namespace Pylae.Data.Services;

public class AuditService : IAuditService
{
    private readonly PylaeMasterDbContext _dbContext;

    public AuditService(PylaeMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        var entity = new AuditEntryEntity
        {
            TimestampUtc = entry.TimestampUtc == default ? DateTime.UtcNow : entry.TimestampUtc,
            SiteCode = entry.SiteCode,
            UserId = entry.UserId,
            Username = entry.Username,
            ActionType = entry.ActionType,
            TargetType = entry.TargetType,
            TargetId = entry.TargetId,
            DetailsJson = entry.DetailsJson
        };

        _dbContext.AuditEntries.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditEntry>> QueryAsync(DateTime? from, DateTime? to, string? actionType, string? targetType, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditEntries.AsNoTracking().AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.TimestampUtc >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.TimestampUtc <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(x => x.ActionType == actionType);
        }

        if (!string.IsNullOrWhiteSpace(targetType))
        {
            query = query.Where(x => x.TargetType == targetType);
        }

        var results = await query
            .OrderByDescending(x => x.TimestampUtc)
            .Select(e => ToDomain(e))
            .ToListAsync(cancellationToken);

        return results;
    }

    private static AuditEntry ToDomain(AuditEntryEntity entity)
    {
        return new AuditEntry
        {
            Id = entity.Id,
            TimestampUtc = entity.TimestampUtc,
            SiteCode = entity.SiteCode,
            UserId = entity.UserId,
            Username = entity.Username,
            ActionType = entity.ActionType,
            TargetType = entity.TargetType,
            TargetId = entity.TargetId,
            DetailsJson = entity.DetailsJson
        };
    }
}
