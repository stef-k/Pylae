using CommunityToolkit.Mvvm.ComponentModel;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.ViewModels;

public partial class UsersViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private List<User> _users = new();

    public UsersViewModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var items = await _userService.GetAllAsync(cancellationToken);
        Users = items.OrderBy(u => u.Username).ToList();
    }

    public async Task<User> SaveAsync(User user, string? password, string? quickCode, CancellationToken cancellationToken = default)
    {
        if (user.Role == UserRole.Admin)
        {
            quickCode = null;
        }

        if (user.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
            throw new InvalidOperationException(Strings.Users_PasswordRequired);
        }

            return await _userService.CreateAsync(user, password, quickCode, cancellationToken);
        }

        var updated = await _userService.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(password))
        {
            await _userService.ChangePasswordAsync(user.Id, password, cancellationToken);
        }

        await _userService.SetQuickCodeAsync(user.Id, quickCode, cancellationToken);
        return updated;
    }

    public Task DeactivateAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _userService.DeactivateAsync(userId, cancellationToken);
    }

    public Task DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _userService.DeleteAsync(userId, cancellationToken);
    }

    public Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default)
    {
        return _userService.ChangePasswordAsync(userId, newPassword, cancellationToken);
    }

    public Task SetQuickCodeAsync(int userId, string? quickCode, CancellationToken cancellationToken = default)
    {
        return _userService.SetQuickCodeAsync(userId, quickCode, cancellationToken);
    }
}
