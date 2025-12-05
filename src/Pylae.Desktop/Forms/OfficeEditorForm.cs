using Pylae.Core.Models;
using Pylae.Desktop.ViewModels;

namespace Pylae.Desktop.Forms;

public partial class OfficeEditorForm : Form
{
    private readonly CatalogsViewModel _viewModel;
    private Office _office;

    public OfficeEditorForm(CatalogsViewModel viewModel)
    {
        _viewModel = viewModel;
        _office = new Office();
        InitializeComponent();
    }

    public void LoadOffice(Office office)
    {
        _office = office;
        codeText.Text = office.Code;
        nameText.Text = office.Name;
        phoneText.Text = office.Phone;
        headNameText.Text = office.HeadFullName;
        headTitleText.Text = office.HeadBusinessTitle;
        headRankText.Text = office.HeadBusinessRank;
        notesText.Text = office.Notes;
        isActiveCheck.Checked = office.IsActive;
        displayOrderNumeric.Value = office.DisplayOrder;
    }

    private async void OnSaveClick(object sender, EventArgs e)
    {
        _office.Code = codeText.Text.Trim();
        _office.Name = nameText.Text.Trim();
        _office.Phone = phoneText.Text.Trim();
        _office.HeadFullName = headNameText.Text.Trim();
        _office.HeadBusinessTitle = headTitleText.Text.Trim();
        _office.HeadBusinessRank = headRankText.Text.Trim();
        _office.Notes = notesText.Text.Trim();
        _office.IsActive = isActiveCheck.Checked;
        _office.DisplayOrder = (int)displayOrderNumeric.Value;

        await _viewModel.SaveOfficeAsync(_office);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
