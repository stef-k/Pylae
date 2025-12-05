using System.Security.Cryptography;
using Pylae.Core.Interfaces;

namespace Pylae.Core.Security;

public class SecretHasher : ISecretHasher
{
    public HashedSecret Hash(string secret, PasswordHashingOptions? options = null, string? purpose = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(secret);

        options ??= new PasswordHashingOptions();
        var saltBytes = RandomNumberGenerator.GetBytes(options.SaltSize);
        var hashBytes = ComputeHash(secret, saltBytes, options, purpose);

        return new HashedSecret(Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes), options.Iterations);
    }

    public bool Verify(string secret, HashedSecret stored, string? purpose = null)
    {
        ArgumentNullException.ThrowIfNull(stored);

        var saltBytes = Convert.FromBase64String(stored.Salt);
        var options = new PasswordHashingOptions
        {
            Iterations = stored.Iterations
        };

        var computed = ComputeHash(secret, saltBytes, options, purpose);
        var storedHashBytes = Convert.FromBase64String(stored.Hash);

        return CryptographicOperations.FixedTimeEquals(computed, storedHashBytes);
    }

    private static byte[] ComputeHash(string secret, byte[] salt, PasswordHashingOptions options, string? purpose)
    {
        var scopedSecret = string.IsNullOrEmpty(purpose) ? secret : $"{purpose}:{secret}";
        return Rfc2898DeriveBytes.Pbkdf2(
            password: scopedSecret,
            salt: salt,
            iterations: options.Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: options.KeySize);
    }
}
