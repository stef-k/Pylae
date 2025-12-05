using Pylae.Core.Security;

namespace Pylae.Core.Interfaces;

public interface ISecretHasher
{
    HashedSecret Hash(string secret, PasswordHashingOptions? options = null, string? purpose = null);

    bool Verify(string secret, HashedSecret stored, string? purpose = null);
}
