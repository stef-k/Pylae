using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _quickCode = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private User? _authenticatedUser;

    public LoginViewModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AuthenticationResult> LoginWithPasswordAsync(CancellationToken cancellationToken = default)
    {
        ErrorMessage = null;
        var result = await _userService.AuthenticateWithPasswordAsync(Username, Password, cancellationToken);

        if (result.Succeeded)
        {
            AuthenticatedUser = result.User;
        }
        else
        {
            ErrorMessage = result.FailureReason ?? Strings.Login_Failed;
        }

        return result;
    }

    public async Task<AuthenticationResult> LoginWithQuickCodeAsync(CancellationToken cancellationToken = default)
    {
        ErrorMessage = null;
        var result = await _userService.AuthenticateWithQuickCodeAsync(Username, QuickCode, cancellationToken);

        if (result.Succeeded)
        {
            AuthenticatedUser = result.User;
        }
        else
        {
            ErrorMessage = result.FailureReason ?? Strings.Login_Failed;
        }

        return result;
    }
}
