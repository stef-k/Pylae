using Pylae.Core.Enums;
using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IVisitService
{
    Task<VisitLogResult> LogVisitAsync(
        int memberNumber,
        VisitDirection direction,
        VisitMethod method,
        string? notes,
        User actor,
        string siteCode,
        string? workstationId,
        int badgeValidityMonths,
        int badgeExpiryWarningDays,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Visit>> GetVisitsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);

    Task UpdateVisitNotesAsync(int visitId, string? notes, int userId, CancellationToken cancellationToken = default);
}
