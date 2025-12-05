using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Models;
using Pylae.Desktop.ViewModels;

namespace Pylae.Desktop.Forms;

public partial class CatalogsForm : Form
{
    private readonly CatalogsViewModel _viewModel;
    private readonly IServiceProvider _services;
    private BindingList<Office> _offices = new();
    private BindingList<MemberType> _memberTypes = new();

    public CatalogsForm(CatalogsViewModel viewModel, IServiceProvider services)
    {
        _viewModel = viewModel;
        _services = services;
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await _viewModel.LoadAsync();
        _offices = new BindingList<Office>(_viewModel.Offices);
        _memberTypes = new BindingList<MemberType>(_viewModel.MemberTypes);
        officesGrid.DataSource = _offices;
        memberTypesGrid.DataSource = _memberTypes;
    }

    private async void OnAddOfficeClick(object sender, EventArgs e)
    {
        var editor = _services.GetRequiredService<OfficeEditorForm>();
        editor.LoadOffice(new Office());
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void OnEditOfficeClick(object sender, EventArgs e)
    {
        if (officesGrid.CurrentRow?.DataBoundItem is not Office office)
        {
            return;
        }

        var editor = _services.GetRequiredService<OfficeEditorForm>();
        editor.LoadOffice(office);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void OnAddMemberTypeClick(object sender, EventArgs e)
    {
        var editor = _services.GetRequiredService<MemberTypeEditorForm>();
        editor.LoadMemberType(new MemberType());
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void OnEditMemberTypeClick(object sender, EventArgs e)
    {
        if (memberTypesGrid.CurrentRow?.DataBoundItem is not MemberType mt)
        {
            return;
        }

        var editor = _services.GetRequiredService<MemberTypeEditorForm>();
        editor.LoadMemberType(mt);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }
}
