using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;
using Pylae.Desktop.Services;
using Pylae.Desktop.ViewModels;
using System.Windows.Forms;
using System.ComponentModel;

namespace Pylae.Desktop.Forms;

public partial class LoginForm : Form
{
    private readonly LoginViewModel _viewModel;
    private readonly CurrentUserService _currentUser;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// When true, the form acts as a modal authenticator and does not open the main window.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DialogMode { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public User? AuthenticatedUser => _viewModel.AuthenticatedUser;

    public LoginForm(LoginViewModel viewModel, IServiceProvider serviceProvider, CurrentUserService currentUser)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _currentUser = currentUser;
        _serviceProvider = serviceProvider;
        Text = $"{Strings.App_Title} - {Strings.App_Subtitle}";
        titleLabel.Text = Strings.Login_Title;
        usernameLabel.Text = Strings.Login_Username;
        passwordLabel.Text = Strings.Login_Password;
        orLabel.Text = Strings.Login_Or;
        quickCodeLabel.Text = Strings.Login_QuickCode;
        loginButton.Text = Strings.Login_Button;
    }

    public void PrepareForReauthentication(User? currentUser = null)
    {
        DialogMode = true;
        usernameTextBox.Text = currentUser?.Username ?? string.Empty;
        passwordTextBox.Text = string.Empty;
        quickCodeTextBox.Text = string.Empty;
        errorLabel.Text = string.Empty;
    }

    private bool _suppressClear;

    private void OnCredentialTextChanged(object? sender, EventArgs e)
    {
        if (_suppressClear) return;
        if (!string.IsNullOrEmpty(usernameTextBox.Text) || !string.IsNullOrEmpty(passwordTextBox.Text))
        {
            _suppressClear = true;
            quickCodeTextBox.Clear();
            _suppressClear = false;
        }
    }

    private void OnQuickCodeTextChanged(object? sender, EventArgs e)
    {
        if (_suppressClear) return;
        if (!string.IsNullOrEmpty(quickCodeTextBox.Text))
        {
            _suppressClear = true;
            // Quick code can work alone or with username
            usernameTextBox.Clear();
            passwordTextBox.Clear();
            _suppressClear = false;
        }
    }

    private async void OnLoginClick(object sender, EventArgs e)
    {
        var useQuickCode = !string.IsNullOrEmpty(quickCodeTextBox.Text.Trim());
        await HandleLoginAsync(useQuickCode);
    }

    private async Task HandleLoginAsync(bool useQuickCode)
    {
        _viewModel.Username = usernameTextBox.Text.Trim();
        _viewModel.Password = passwordTextBox.Text;
        _viewModel.QuickCode = quickCodeTextBox.Text.Trim();

        var result = useQuickCode
            ? await _viewModel.LoginWithQuickCodeAsync()
            : await _viewModel.LoginWithPasswordAsync();

        if (!result.Succeeded || result.User is null)
        {
        errorLabel.Text = _viewModel.ErrorMessage ?? Strings.Lock_InvalidCredentials;
            return;
        }

        _currentUser.CurrentUser = result.User;
        if (DialogMode)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        OpenMainForm(result.User);
    }

    private void OpenMainForm(User user)
    {
        var mainForm = _serviceProvider.GetRequiredService<MainForm>();
        mainForm.SetCurrentUser(user);

        Hide();
        mainForm.FormClosed += (_, _) => Close();
        mainForm.Show();
    }
}
