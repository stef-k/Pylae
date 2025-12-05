using Pylae.Core.Models;

namespace Pylae.Desktop.Services;

public class CurrentUserService
{
    public event Action<User?>? CurrentUserChanged;

    private User? _currentUser;

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            CurrentUserChanged?.Invoke(_currentUser);
        }
    }
}
