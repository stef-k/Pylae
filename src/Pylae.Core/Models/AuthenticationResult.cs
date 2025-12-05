namespace Pylae.Core.Models;

public class AuthenticationResult
{
    public AuthenticationResult(bool succeeded, User? user, string? failureReason = null)
    {
        Succeeded = succeeded;
        User = user;
        FailureReason = failureReason;
    }

    public bool Succeeded { get; }

    public User? User { get; }

    public string? FailureReason { get; }
}
