namespace Pylae.Core.Security;

public class HashedSecret
{
    public HashedSecret(string hash, string salt, int iterations)
    {
        Hash = hash;
        Salt = salt;
        Iterations = iterations;
    }

    public string Hash { get; }

    public string Salt { get; }

    public int Iterations { get; }
}
