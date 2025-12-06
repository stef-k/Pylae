using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class CatalogsViewModel : ObservableObject
{
    private readonly IMemberTypeService _memberTypeService;

    [ObservableProperty]
    private List<MemberType> _memberTypes = new();

    public CatalogsViewModel(IMemberTypeService memberTypeService)
    {
        _memberTypeService = memberTypeService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        MemberTypes = (await _memberTypeService.GetAllAsync(cancellationToken)).ToList();
    }

    public async Task<MemberType> SaveMemberTypeAsync(MemberType memberType, CancellationToken cancellationToken = default)
    {
        if (memberType.Id == 0)
        {
            return await _memberTypeService.CreateAsync(memberType, cancellationToken);
        }

        return await _memberTypeService.UpdateAsync(memberType, cancellationToken);
    }
}
