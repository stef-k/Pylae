using Pylae.Core.Models;
using Pylae.Desktop.ViewModels;

namespace Pylae.Desktop.Forms;

public partial class MemberTypeEditorForm : Form
{
    private readonly CatalogsViewModel _viewModel;
    private MemberType _memberType = new();

    public MemberTypeEditorForm(CatalogsViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
    }

    public void LoadMemberType(MemberType memberType)
    {
        _memberType = memberType;
        codeText.Text = memberType.Code;
        nameText.Text = memberType.DisplayName;
        descriptionText.Text = memberType.Description;
        displayOrderNumeric.Value = memberType.DisplayOrder;
        isActiveCheck.Checked = memberType.IsActive;
    }

    private async void OnSaveClick(object sender, EventArgs e)
    {
        _memberType.Code = codeText.Text.Trim();
        _memberType.DisplayName = nameText.Text.Trim();
        _memberType.Description = descriptionText.Text.Trim();
        _memberType.DisplayOrder = (int)displayOrderNumeric.Value;
        _memberType.IsActive = isActiveCheck.Checked;

        await _viewModel.SaveMemberTypeAsync(_memberType);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
