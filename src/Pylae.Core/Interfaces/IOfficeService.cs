using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IOfficeService
{
    Task<IReadOnlyCollection<Office>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Office> CreateAsync(Office office, CancellationToken cancellationToken = default);

    Task<Office> UpdateAsync(Office office, CancellationToken cancellationToken = default);
}
