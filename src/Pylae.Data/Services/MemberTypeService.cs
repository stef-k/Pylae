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
        entity.Code = await GenerateUniqueCodeAsync(memberType.DisplayName, null, cancellationToken);
        entity.DisplayOrder = await GetNextDisplayOrderAsync(cancellationToken);
        entity.CreatedAtUtc = DateTime.UtcNow;
        _dbContext.MemberTypes.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private async Task<int> GetNextDisplayOrderAsync(CancellationToken cancellationToken)
    {
        var maxOrder = await _dbContext.MemberTypes.MaxAsync(m => (int?)m.DisplayOrder, cancellationToken);
        return (maxOrder ?? 0) + 1;
    }

    public async Task<MemberType> UpdateAsync(MemberType memberType, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MemberTypes.FirstOrDefaultAsync(m => m.Id == memberType.Id, cancellationToken)
            ?? throw new InvalidOperationException("Member type not found.");

        // Only regenerate code if DisplayName changed
        if (entity.DisplayName != memberType.DisplayName)
        {
            entity.Code = await GenerateUniqueCodeAsync(memberType.DisplayName, memberType.Id, cancellationToken);
        }
        entity.DisplayName = memberType.DisplayName;
        entity.Description = memberType.Description;
        entity.IsActive = memberType.IsActive;
        entity.DisplayOrder = memberType.DisplayOrder;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private async Task<string> GenerateUniqueCodeAsync(string displayName, int? excludeId, CancellationToken cancellationToken)
    {
        // Generate base code: lowercase, trim, replace spaces with hyphens, remove special chars
        var baseCode = displayName.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-");

        // Remove any characters that aren't alphanumeric or hyphen
        baseCode = System.Text.RegularExpressions.Regex.Replace(baseCode, @"[^a-z0-9\-]", "");

        if (string.IsNullOrEmpty(baseCode))
            baseCode = "type";

        // Check if code exists
        var existsQuery = _dbContext.MemberTypes.Where(m => m.Code == baseCode);
        if (excludeId.HasValue)
            existsQuery = existsQuery.Where(m => m.Id != excludeId.Value);

        if (!await existsQuery.AnyAsync(cancellationToken))
            return baseCode;

        // Code exists, find a unique one with suffix
        var suffix = 1;
        while (true)
        {
            var candidateCode = $"{baseCode}-{suffix}";
            var candidateQuery = _dbContext.MemberTypes.Where(m => m.Code == candidateCode);
            if (excludeId.HasValue)
                candidateQuery = candidateQuery.Where(m => m.Id != excludeId.Value);

            if (!await candidateQuery.AnyAsync(cancellationToken))
                return candidateCode;

            suffix++;
            if (suffix > 100) // Safety limit
                return $"{baseCode}-{Guid.NewGuid():N}".Substring(0, 50);
        }
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
