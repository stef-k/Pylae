using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class MembersViewModel : ObservableObject
{
    private readonly IMemberService _memberService;
    private readonly IMemberTypeService _memberTypeService;

    [ObservableProperty]
    private List<Member> _members = new();

    [ObservableProperty]
    private List<MemberType> _memberTypes = new();

    public MembersViewModel(IMemberService memberService, IMemberTypeService memberTypeService)
    {
        _memberService = memberService;
        _memberTypeService = memberTypeService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var items = await _memberService.SearchAsync(null, cancellationToken);
        Members = items.ToList();

        MemberTypes = (await _memberTypeService.GetAllAsync(cancellationToken)).ToList();
    }

    public async Task<Member> SaveAsync(Member member, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(member.Id))
        {
            member.Id = Guid.NewGuid().ToString();
            return await _memberService.CreateAsync(member, cancellationToken);
        }

        return await _memberService.UpdateAsync(member, cancellationToken);
    }
}
