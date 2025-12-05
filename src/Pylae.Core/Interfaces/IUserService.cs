using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IUserService
{
    Task<AuthenticationResult> AuthenticateWithPasswordAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> AuthenticateWithQuickCodeAsync(string username, string quickCode, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User> CreateAsync(User user, string password, string? quickCode, CancellationToken cancellationToken = default);

    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default);

    Task SetQuickCodeAsync(int userId, string? quickCode, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int userId, CancellationToken cancellationToken = default);

    Task DeleteAsync(int userId, CancellationToken cancellationToken = default);

    Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken = default);
}
