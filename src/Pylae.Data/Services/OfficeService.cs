using Microsoft.EntityFrameworkCore;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using OfficeEntity = Pylae.Data.Entities.Master.Office;

namespace Pylae.Data.Services;

public class OfficeService : IOfficeService
{
    private readonly PylaeMasterDbContext _dbContext;

    public OfficeService(PylaeMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Office>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var offices = await _dbContext.Offices.AsNoTracking()
            .OrderBy(o => o.DisplayOrder)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken);

        return offices.Select(ToDomain).ToList();
    }

    public async Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Offices.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Office> CreateAsync(Office office, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(office);
        entity.CreatedAtUtc = DateTime.UtcNow;
        _dbContext.Offices.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<Office> UpdateAsync(Office office, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Offices.FirstOrDefaultAsync(o => o.Id == office.Id, cancellationToken)
            ?? throw new InvalidOperationException("Office not found.");

        entity.Code = office.Code;
        entity.Name = office.Name;
        entity.Phone = office.Phone;
        entity.HeadFullName = office.HeadFullName;
        entity.HeadBusinessTitle = office.HeadBusinessTitle;
        entity.HeadBusinessRank = office.HeadBusinessRank;
        entity.Notes = office.Notes;
        entity.IsActive = office.IsActive;
        entity.DisplayOrder = office.DisplayOrder;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private static OfficeEntity MapToEntity(Office office)
    {
        return new OfficeEntity
        {
            Id = office.Id,
            Code = office.Code,
            Name = office.Name,
            Phone = office.Phone,
            HeadFullName = office.HeadFullName,
            HeadBusinessTitle = office.HeadBusinessTitle,
            HeadBusinessRank = office.HeadBusinessRank,
            Notes = office.Notes,
            IsActive = office.IsActive,
            DisplayOrder = office.DisplayOrder,
            CreatedAtUtc = office.CreatedAtUtc,
            UpdatedAtUtc = office.UpdatedAtUtc
        };
    }

    private static Office ToDomain(OfficeEntity entity)
    {
        return new Office
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Phone = entity.Phone,
            HeadFullName = entity.HeadFullName,
            HeadBusinessTitle = entity.HeadBusinessTitle,
            HeadBusinessRank = entity.HeadBusinessRank,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            DisplayOrder = entity.DisplayOrder,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
