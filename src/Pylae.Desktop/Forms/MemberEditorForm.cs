using System.IO;
using Pylae.Desktop.Resources;
using Pylae.Core.Models;
using Pylae.Data.Context;
using Pylae.Desktop.ViewModels;

namespace Pylae.Desktop.Forms;

public partial class MemberEditorForm : Form
{
    private readonly MembersViewModel _viewModel;
    private readonly DatabaseOptions _dbOptions;
    private Member _member;

    public MemberEditorForm(MembersViewModel viewModel, DatabaseOptions dbOptions)
    {
        _viewModel = viewModel;
        _dbOptions = dbOptions;
        _member = new Member();
        InitializeComponent();
    }

    public void LoadMember(Member member)
    {
        _member = member;
        memberNumberTextBox.Text = member.MemberNumber.ToString();
        firstNameTextBox.Text = member.FirstName;
        lastNameTextBox.Text = member.LastName;
        businessRankTextBox.Text = member.BusinessRank;
        personalIdTextBox.Text = member.PersonalIdNumber;
        businessIdTextBox.Text = member.BusinessIdNumber;
        isPermanentCheckBox.Checked = member.IsPermanentStaff;
        officeTextBox.Text = member.Office ?? string.Empty;

        memberTypeCombo.DataSource = _viewModel.MemberTypes;
        memberTypeCombo.DisplayMember = nameof(MemberType.DisplayName);
        memberTypeCombo.ValueMember = nameof(MemberType.Id);
        memberTypeCombo.SelectedValue = member.MemberTypeId ?? 0;
        photoLabel.Text = string.IsNullOrWhiteSpace(member.PhotoFileName) ? Strings.Photo_None : member.PhotoFileName;
    }

    private async void OnSaveClick(object sender, EventArgs e)
    {
        if (!int.TryParse(memberNumberTextBox.Text, out var memberNumber))
        {
            MessageBox.Show(Strings.Members_NumberInvalid, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _member.MemberNumber = memberNumber;
        _member.FirstName = firstNameTextBox.Text.Trim();
        _member.LastName = lastNameTextBox.Text.Trim();
        _member.BusinessRank = businessRankTextBox.Text.Trim();
        _member.PersonalIdNumber = personalIdTextBox.Text.Trim();
        _member.BusinessIdNumber = businessIdTextBox.Text.Trim();
        _member.IsPermanentStaff = isPermanentCheckBox.Checked;
        _member.Office = string.IsNullOrWhiteSpace(officeTextBox.Text) ? null : officeTextBox.Text.Trim();
        _member.MemberTypeId = memberTypeCombo.SelectedValue is int memberTypeId ? memberTypeId : null;

        await _viewModel.SaveAsync(_member);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnChoosePhotoClick(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = Strings.Photo_Filter,
            Title = Strings.Photo_SelectTitle
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var photosDir = _dbOptions.GetPhotosPath();
            Directory.CreateDirectory(photosDir);

            var extension = Path.GetExtension(dialog.FileName);
            var fileName = $"{_member.Id}{extension}";
            var destination = Path.Combine(photosDir, fileName);
            File.Copy(dialog.FileName, destination, overwrite: true);
            _member.PhotoFileName = fileName;
            photoLabel.Text = fileName;
        }
    }

    private void OnClearPhotoClick(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_member.PhotoFileName))
        {
            var path = Path.Combine(_dbOptions.GetPhotosPath(), _member.PhotoFileName);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // ignore delete failures
                }
            }
        }

        _member.PhotoFileName = null;
        photoLabel.Text = Strings.Photo_None;
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
