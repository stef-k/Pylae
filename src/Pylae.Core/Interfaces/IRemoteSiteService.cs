using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IRemoteSiteService
{
    Task<IReadOnlyList<RemoteSite>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RemoteSite?> GetBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default);

    Task<RemoteSite> CreateAsync(RemoteSite site, CancellationToken cancellationToken = default);

    Task<RemoteSite> UpdateAsync(RemoteSite site, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default);
}
