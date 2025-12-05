using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IMemberTypeService
{
    Task<IReadOnlyCollection<MemberType>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MemberType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<MemberType> CreateAsync(MemberType memberType, CancellationToken cancellationToken = default);

    Task<MemberType> UpdateAsync(MemberType memberType, CancellationToken cancellationToken = default);
}
