using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class VisitsViewModel : ObservableObject
{
    private readonly IVisitService _visitService;

    [ObservableProperty]
    private List<Visit> _visits = new();

    public VisitsViewModel(IVisitService visitService)
    {
        _visitService = visitService;
    }

    public async Task LoadAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var items = await _visitService.GetVisitsAsync(from, to, cancellationToken);
        Visits = items.ToList();
    }

    public async Task UpdateNotesAsync(int visitId, string? notes, int userId, CancellationToken cancellationToken = default)
    {
        await _visitService.UpdateVisitNotesAsync(visitId, notes, userId, cancellationToken);
    }
}
