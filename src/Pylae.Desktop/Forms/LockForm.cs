using Pylae.Core.Interfaces;
using Pylae.Core.Enums;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;
using Pylae.Desktop.Services;

namespace Pylae.Desktop.Forms;

public partial class LockForm : Form
{
    private readonly IUserService _userService;
    private readonly CurrentUserService _currentUserService;
    private User? _lockedUser;

    public LockForm(IUserService userService, CurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        InitializeComponent();
        promptLabel.Text = Strings.Lock_Prompt;
        unlockButton.Text = Strings.Button_Unlock;
        cancelButton.Text = Strings.Button_Cancel;
        switchUserButton.Text = Strings.Button_SwitchUser;
        passwordLabel.Text = Strings.Login_Password;
        quickCodeLabel.Text = Strings.Login_QuickCode;
        UpdateCurrentUser(_currentUserService.CurrentUser);
    }

    private async void OnUnlockClick(object sender, EventArgs e)
    {
        var current = _lockedUser ?? _currentUserService.CurrentUser;
        if (current is null)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        var result = !string.IsNullOrWhiteSpace(passwordText.Text)
            ? await _userService.AuthenticateWithPasswordAsync(current.Username, passwordText.Text)
            : await _userService.AuthenticateWithQuickCodeAsync(current.Username, quickCodeText.Text);

        if (result.Succeeded && result.User is not null)
        {
            _currentUserService.CurrentUser = result.User;
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        MessageBox.Show(Strings.Lock_InvalidCredentials, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void OnSwitchUserClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Retry;
        Close();
    }

    public void SetCurrentUser(User? user)
    {
        _lockedUser = user;
        UpdateCurrentUser(user);
    }

    private void UpdateCurrentUser(User? user)
    {
        if (user is null)
        {
            currentUserLabel.Text = Strings.Lock_NoUser;
            quickCodeText.Enabled = true;
            return;
        }

        currentUserLabel.Text = string.Format(Strings.Lock_CurrentUser, $"{user.FirstName} {user.LastName} ({user.Username})");
        quickCodeText.Enabled = user.Role != UserRole.Admin;
    }
}
