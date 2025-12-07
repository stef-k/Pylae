using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Core.Security;
using Pylae.Data.Context;
using UserEntity = Pylae.Data.Entities.Master.User;

namespace Pylae.Data.Services;

public class UserService : IUserService
{
    private readonly PylaeMasterDbContext _dbContext;
    private readonly ISecretHasher _secretHasher;
    private readonly IClock _clock;
    private readonly ILogger<UserService>? _logger;

    public UserService(PylaeMasterDbContext dbContext, ISecretHasher secretHasher, IClock clock, ILogger<UserService>? logger = null)
    {
        _dbContext = dbContext;
        _secretHasher = secretHasher;
        _clock = clock;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateWithPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);
        if (user is null)
        {
            return new AuthenticationResult(false, null, "Invalid credentials.");
        }

        if (!HasPassword(user) || !VerifyPassword(user, password))
        {
            _logger?.LogWarning("Failed password login for user {Username}", username);
            return new AuthenticationResult(false, null, "Invalid credentials.");
        }

        user.LastLoginAtUtc = _clock.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(true, ToDomain(user));
    }

    public async Task<AuthenticationResult> AuthenticateWithQuickCodeAsync(string username, string quickCode, CancellationToken cancellationToken = default)
    {
        Entities.Master.User? user;

        if (!string.IsNullOrWhiteSpace(username))
        {
            // Username provided - look up specific user
            user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);
            if (user is null)
            {
                return new AuthenticationResult(false, null, "Invalid credentials.");
            }
        }
        else
        {
            // No username - search all active non-admin users with quick codes
            var candidates = await _dbContext.Users
                .Where(u => u.IsActive && u.Role != UserRole.Admin && u.QuickCodeHash != null)
                .ToListAsync(cancellationToken);

            user = candidates.FirstOrDefault(u => VerifyQuickCode(u, quickCode));
            if (user is null)
            {
                _logger?.LogWarning("Failed quick code login - no matching user found");
                return new AuthenticationResult(false, null, "Invalid credentials.");
            }
        }

        if (user.Role == UserRole.Admin)
        {
            return new AuthenticationResult(false, null, "QuickCode is not allowed for admins.");
        }

        if (!HasQuickCode(user) || !VerifyQuickCode(user, quickCode))
        {
            _logger?.LogWarning("Failed quick code login for user {Username}", user.Username);
            return new AuthenticationResult(false, null, "Invalid credentials.");
        }

        user.LastLoginAtUtc = _clock.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(true, ToDomain(user));
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<User> CreateAsync(User user, string password, string? quickCode, CancellationToken cancellationToken = default)
    {
        await EnsureUsernameAvailable(user.Username, cancellationToken);

        var entity = MapToEntity(user);
        entity.CreatedAtUtc = _clock.UtcNow;
        entity.IsActive = true;

        SetPassword(entity, password);
        SetQuickCode(entity, quickCode);

        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        EnsureSystemAccountProtected(entity, user);
        EnsureRoleChangeAllowed(entity, user);

        entity.FirstName = user.FirstName;
        entity.LastName = user.LastName;
        entity.Username = user.Username;
        entity.IsShared = user.IsShared;
        entity.IsActive = user.IsActive;
        entity.Role = user.Role;

        await EnsureUsernameAvailable(entity.Username, cancellationToken, entity.Id);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task ChangePasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        SetPassword(user, newPassword);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetQuickCodeAsync(int userId, string? quickCode, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        if (user.Role == UserRole.Admin && !string.IsNullOrWhiteSpace(quickCode))
        {
            throw new InvalidOperationException("Admins cannot use QuickCode.");
        }

        SetQuickCode(user, quickCode);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return;
        }

        EnsureSystemAccountProtected(user);
        await EnsureAdminWillRemain(user, cancellationToken);

        user.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return;
        }

        EnsureSystemAccountProtected(user);
        await EnsureAdminWillRemain(user, cancellationToken);

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.CountAsync(u => u.Role == UserRole.Admin && u.IsActive, cancellationToken);
    }

    private async Task EnsureUsernameAvailable(string username, CancellationToken cancellationToken, int? currentId = null)
    {
        var exists = await _dbContext.Users.AnyAsync(
            u => u.Username == username && (!currentId.HasValue || u.Id != currentId.Value),
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Username is already in use.");
        }
    }

    private static UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            IsShared = user.IsShared,
            IsSystem = user.IsSystem,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }

    private void SetPassword(UserEntity user, string password)
    {
        var hashed = _secretHasher.Hash(password, null, SecretPurposes.Password);
        user.PasswordHash = hashed.Hash;
        user.PasswordSalt = hashed.Salt;
        user.PasswordIterations = hashed.Iterations;
    }

    private void SetQuickCode(UserEntity user, string? quickCode)
    {
        if (string.IsNullOrWhiteSpace(quickCode))
        {
            user.QuickCodeHash = null;
            user.QuickCodeSalt = null;
            user.QuickCodeIterations = 0;
            return;
        }

        if (!QuickCodeValidator.IsValid(quickCode, out var validationError))
        {
            throw new InvalidOperationException(validationError);
        }

        var hashed = _secretHasher.Hash(quickCode, null, SecretPurposes.QuickCode);
        user.QuickCodeHash = hashed.Hash;
        user.QuickCodeSalt = hashed.Salt;
        user.QuickCodeIterations = hashed.Iterations;
    }

    private static bool HasPassword(UserEntity user)
    {
        return !string.IsNullOrWhiteSpace(user.PasswordHash) && !string.IsNullOrWhiteSpace(user.PasswordSalt);
    }

    private bool VerifyPassword(UserEntity user, string password)
    {
        if (!HasPassword(user))
        {
            return false;
        }

        var stored = new HashedSecret(user.PasswordHash!, user.PasswordSalt!, user.PasswordIterations);
        return _secretHasher.Verify(password, stored, SecretPurposes.Password);
    }

    private static bool HasQuickCode(UserEntity user)
    {
        return !string.IsNullOrWhiteSpace(user.QuickCodeHash) && !string.IsNullOrWhiteSpace(user.QuickCodeSalt);
    }

    private bool VerifyQuickCode(UserEntity user, string quickCode)
    {
        if (!HasQuickCode(user))
        {
            return false;
        }

        var stored = new HashedSecret(user.QuickCodeHash!, user.QuickCodeSalt!, user.QuickCodeIterations);
        return _secretHasher.Verify(quickCode, stored, SecretPurposes.QuickCode);
    }

    private static void EnsureSystemAccountProtected(UserEntity current, User updated)
    {
        if (current.IsSystem && updated.IsActive == false)
        {
            throw new InvalidOperationException("Protected system admin cannot be deactivated.");
        }

        if (current.IsSystem && updated.Role != UserRole.Admin)
        {
            throw new InvalidOperationException("Protected system admin role cannot be changed.");
        }
    }

    private static void EnsureRoleChangeAllowed(UserEntity current, User updated)
    {
        if (current.IsSystem && updated.Role != UserRole.Admin)
        {
            throw new InvalidOperationException("System admin must remain admin.");
        }
    }

    private static void EnsureSystemAccountProtected(UserEntity current)
    {
        if (current.IsSystem)
        {
            throw new InvalidOperationException("Protected system admin cannot be removed or deactivated.");
        }
    }

    private async Task EnsureAdminWillRemain(UserEntity candidate, CancellationToken cancellationToken)
    {
        if (candidate.Role != UserRole.Admin || !candidate.IsActive)
        {
            return;
        }

        var adminCount = await CountActiveAdminsAsync(cancellationToken);
        if (adminCount <= 1)
        {
            throw new InvalidOperationException("Operation blocked: at least one active admin is required.");
        }
    }

    private static User ToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Username = entity.Username,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Role = entity.Role,
            IsShared = entity.IsShared,
            IsSystem = entity.IsSystem,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            LastLoginAtUtc = entity.LastLoginAtUtc
        };
    }
}
