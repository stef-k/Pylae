using Microsoft.EntityFrameworkCore;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using MemberTypeEntity = Pylae.Data.Entities.Master.MemberType;

namespace Pylae.Data.Services;

public class MemberTypeService : IMemberTypeService
{
    private readonly PylaeMasterDbContext _dbContext;

    public MemberTypeService(PylaeMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<MemberType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.MemberTypes.AsNoTracking()
            .OrderBy(m => m.DisplayOrder)
            .ThenBy(m => m.DisplayName)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<MemberType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MemberTypes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<MemberType> CreateAsync(MemberType memberType, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(memberType);
        entity.CreatedAtUtc = DateTime.UtcNow;
        _dbContext.MemberTypes.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<MemberType> UpdateAsync(MemberType memberType, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MemberTypes.FirstOrDefaultAsync(m => m.Id == memberType.Id, cancellationToken)
            ?? throw new InvalidOperationException("Member type not found.");

        entity.Code = memberType.Code;
        entity.DisplayName = memberType.DisplayName;
        entity.Description = memberType.Description;
        entity.IsActive = memberType.IsActive;
        entity.DisplayOrder = memberType.DisplayOrder;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private static MemberTypeEntity MapToEntity(MemberType memberType)
    {
        return new MemberTypeEntity
        {
            Id = memberType.Id,
            Code = memberType.Code,
            DisplayName = memberType.DisplayName,
            Description = memberType.Description,
            IsActive = memberType.IsActive,
            DisplayOrder = memberType.DisplayOrder,
            CreatedAtUtc = memberType.CreatedAtUtc,
            UpdatedAtUtc = memberType.UpdatedAtUtc
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
