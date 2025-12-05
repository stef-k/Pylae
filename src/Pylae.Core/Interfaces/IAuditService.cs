using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditEntry>> QueryAsync(
        DateTime? from,
        DateTime? to,
        string? actionType,
        string? targetType,
        CancellationToken cancellationToken = default);
}
