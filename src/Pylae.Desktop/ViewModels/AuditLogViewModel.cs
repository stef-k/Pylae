using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class AuditLogViewModel : ObservableObject
{
    private readonly IAuditService _auditService;

    [ObservableProperty]
    private List<AuditEntry> _entries = new();

    public AuditLogViewModel(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task LoadAsync(DateTime? from, DateTime? to, string? actionType, string? targetType, CancellationToken cancellationToken = default)
    {
        var result = await _auditService.QueryAsync(from, to, actionType, targetType, cancellationToken);
        Entries = result.ToList();
    }
}
