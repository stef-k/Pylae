using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class CatalogsViewModel : ObservableObject
{
    private readonly IOfficeService _officeService;
    private readonly IMemberTypeService _memberTypeService;

    [ObservableProperty]
    private List<Office> _offices = new();

    [ObservableProperty]
    private List<MemberType> _memberTypes = new();

    public CatalogsViewModel(IOfficeService officeService, IMemberTypeService memberTypeService)
    {
        _officeService = officeService;
        _memberTypeService = memberTypeService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Offices = (await _officeService.GetAllAsync(cancellationToken)).ToList();
        MemberTypes = (await _memberTypeService.GetAllAsync(cancellationToken)).ToList();
    }

    public async Task<Office> SaveOfficeAsync(Office office, CancellationToken cancellationToken = default)
    {
        if (office.Id == 0)
        {
            return await _officeService.CreateAsync(office, cancellationToken);
        }

        return await _officeService.UpdateAsync(office, cancellationToken);
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
