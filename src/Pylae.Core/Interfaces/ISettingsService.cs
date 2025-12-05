using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface ISettingsService
{
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);

    Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpsertAsync(IEnumerable<Setting> settings, CancellationToken cancellationToken = default);

    Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default);

    Task EnsureDefaultsAsync(CancellationToken cancellationToken = default);
}
