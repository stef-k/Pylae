using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;
using Pylae.Desktop.ViewModels;

namespace Pylae.Desktop.Forms;

public partial class CatalogsForm : Form
{
    private readonly CatalogsViewModel _viewModel;
    private readonly IServiceProvider _services;
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
        _memberTypes = new BindingList<MemberType>(_viewModel.MemberTypes);
        memberTypesGrid.DataSource = _memberTypes;

        // Hide internal columns
        if (memberTypesGrid.Columns["Id"] is { } idCol)
            idCol.Visible = false;
        if (memberTypesGrid.Columns["Code"] is { } codeCol)
            codeCol.Visible = false;
        if (memberTypesGrid.Columns["CreatedAtUtc"] is { } createdCol)
            createdCol.Visible = false;
        if (memberTypesGrid.Columns["UpdatedAtUtc"] is { } updatedCol)
            updatedCol.Visible = false;

        // Localize column headers
        if (memberTypesGrid.Columns["DisplayName"] is { } nameCol)
            nameCol.HeaderText = Strings.Grid_DisplayName;
        if (memberTypesGrid.Columns["Description"] is { } descCol)
            descCol.HeaderText = Strings.Grid_Description;
        if (memberTypesGrid.Columns["DisplayOrder"] is { } orderCol)
            orderCol.HeaderText = Strings.Grid_DisplayOrder;
        if (memberTypesGrid.Columns["IsActive"] is { } activeCol)
            activeCol.HeaderText = Strings.Grid_IsActive;
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
