using Microsoft.EntityFrameworkCore;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using RemoteSiteEntity = Pylae.Data.Entities.Master.RemoteSite;

namespace Pylae.Data.Services;

public class RemoteSiteService : IRemoteSiteService
{
    private readonly PylaeMasterDbContext _dbContext;

    public RemoteSiteService(PylaeMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<RemoteSite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.RemoteSites.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(MapToModel).ToList();
    }

    public async Task<RemoteSite?> GetBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RemoteSites.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteCode == siteCode, cancellationToken);
        return entity is null ? null : MapToModel(entity);
    }

    public async Task<RemoteSite> CreateAsync(RemoteSite site, CancellationToken cancellationToken = default)
    {
        var entity = new RemoteSiteEntity
        {
            SiteCode = site.SiteCode,
            DisplayName = site.DisplayName,
            Host = site.Host,
            Port = site.Port,
            ApiKey = site.ApiKey,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.RemoteSites.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToModel(entity);
    }

    public async Task<RemoteSite> UpdateAsync(RemoteSite site, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RemoteSites.FirstOrDefaultAsync(x => x.Id == site.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Remote site with ID {site.Id} not found.");

        entity.SiteCode = site.SiteCode;
        entity.DisplayName = site.DisplayName;
        entity.Host = site.Host;
        entity.Port = site.Port;
        entity.ApiKey = site.ApiKey;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToModel(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RemoteSites.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _dbContext.RemoteSites.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RemoteSites.FirstOrDefaultAsync(x => x.SiteCode == siteCode, cancellationToken);
        if (entity is not null)
        {
            _dbContext.RemoteSites.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static RemoteSite MapToModel(RemoteSiteEntity entity) => new()
    {
        Id = entity.Id,
        SiteCode = entity.SiteCode,
        DisplayName = entity.DisplayName,
        Host = entity.Host,
        Port = entity.Port,
        ApiKey = entity.ApiKey,
        CreatedAtUtc = entity.CreatedAtUtc,
        UpdatedAtUtc = entity.UpdatedAtUtc
    };
}
