using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportMembersAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default);

    Task<byte[]> ExportVisitsAsync(IEnumerable<Visit> visits, CancellationToken cancellationToken = default);

    Task<byte[]> ExportAuditAsync(IEnumerable<AuditEntry> entries, CancellationToken cancellationToken = default);

    Task<byte[]> ExportMembersJsonAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default);

    Task<byte[]> ExportVisitsJsonAsync(IEnumerable<Visit> visits, CancellationToken cancellationToken = default);

    Task<byte[]> ExportAuditJsonAsync(IEnumerable<AuditEntry> entries, CancellationToken cancellationToken = default);
}
