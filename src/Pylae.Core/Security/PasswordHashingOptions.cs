namespace Pylae.Core.Security;

public class PasswordHashingOptions
{
    public int Iterations { get; init; } = 120_000;

    public int SaltSize { get; init; } = 16;

    public int KeySize { get; init; } = 32;
}
