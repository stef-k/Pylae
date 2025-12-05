using Pylae.Core.Enums;

namespace Pylae.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public int PasswordIterations { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public string? QuickCodeHash { get; set; }
    public string? QuickCodeSalt { get; set; }
    public int QuickCodeIterations { get; set; }
    public bool IsShared { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; set; }
}
