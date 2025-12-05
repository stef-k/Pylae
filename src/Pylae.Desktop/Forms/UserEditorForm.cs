using Pylae.Core.Enums;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;
using Pylae.Desktop.ViewModels;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

public partial class UserEditorForm : Form
{
    private readonly UsersViewModel _viewModel;
    private User _user = new();

    public UserEditorForm(UsersViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        roleCombo.DataSource = Enum.GetValues(typeof(UserRole));
        saveButton.Text = Strings.Button_Save;
        cancelButton.Text = Strings.Button_Cancel;
        Text = Strings.Tab_Users;
        usernameLabel.Text = Strings.Users_Username;
        firstNameLabel.Text = Strings.Users_FirstName;
        lastNameLabel.Text = Strings.Users_LastName;
        roleLabel.Text = Strings.Users_Role;
        sharedCheck.Text = Strings.Users_IsShared;
        activeCheck.Text = Strings.Users_IsActive;
        passwordLabel.Text = Strings.Users_Password;
        quickCodeLabel.Text = Strings.Login_QuickCode;
    }

    public void LoadUser(User user)
    {
        _user = user;
        usernameText.Text = user.Username;
        firstNameText.Text = user.FirstName;
        lastNameText.Text = user.LastName;
        roleCombo.SelectedItem = user.Role;
        sharedCheck.Checked = user.IsShared;
        activeCheck.Checked = user.IsActive;
        passwordText.Text = string.Empty;
        quickCodeText.Text = string.Empty;
        ApplyRoleRules();
    }

    private async void OnSaveClick(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(usernameText.Text))
        {
            MessageBox.Show(Strings.Users_UsernameRequired, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _user.Username = usernameText.Text.Trim();
        _user.FirstName = firstNameText.Text.Trim();
        _user.LastName = lastNameText.Text.Trim();
        _user.Role = (UserRole)roleCombo.SelectedItem!;
        _user.IsShared = sharedCheck.Checked;
        _user.IsActive = activeCheck.Checked;

        var password = string.IsNullOrWhiteSpace(passwordText.Text) ? null : passwordText.Text;
        var quickCode = string.IsNullOrWhiteSpace(quickCodeText.Text) ? null : quickCodeText.Text;

        try
        {
            await _viewModel.SaveAsync(_user, password, quickCode);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void OnRoleChanged(object? sender, EventArgs e)
    {
        ApplyRoleRules();
    }

    private void ApplyRoleRules()
    {
        var selectedRole = (UserRole)roleCombo.SelectedItem!;
        var isAdmin = selectedRole == UserRole.Admin;
        quickCodeText.Enabled = !isAdmin;
        if (isAdmin)
        {
            quickCodeText.Text = string.Empty;
            sharedCheck.Checked = false;
        }
    }
}
