using Pylae.Sync.Models;

namespace Pylae.Sync.Client;

public interface IRemoteSiteClient
{
    Task<SyncInfoResponse?> GetInfoAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Pylae.Core.Models.Visit>> GetVisitsAsync(RemoteSiteConfig config, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);

    Task<byte[]?> GetFullVisitsDatabaseAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default);

    Task<MasterPackage?> GetMasterPackageAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default);

    Task<bool> UploadMasterPackageAsync(RemoteSiteConfig config, MasterPackage package, CancellationToken cancellationToken = default);
}
