namespace Pylae.Core.Security;

/// <summary>
/// Validates password strength according to security policy.
/// </summary>
public static class PasswordValidator
{
    private const int MinimumLength = 8;

    /// <summary>
    /// Validates that a password meets minimum strength requirements.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <param name="errorMessage">The error message if validation fails.</param>
    /// <returns>True if password is valid, false otherwise.</returns>
    public static bool IsValid(string? password, out string errorMessage)
    {
        if (string.IsNullOrEmpty(password))
        {
            errorMessage = "Password cannot be empty.";
            return false;
        }

        if (password.Length < MinimumLength)
        {
            errorMessage = $"Password must be at least {MinimumLength} characters long.";
            return false;
        }

        var hasUpper = false;
        var hasLower = false;
        var hasDigit = false;
        var hasSpecial = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (char.IsSymbol(c) || char.IsPunctuation(c)) hasSpecial = true;
        }

        var missingRequirements = new List<string>();

        if (!hasUpper) missingRequirements.Add("uppercase letter");
        if (!hasLower) missingRequirements.Add("lowercase letter");
        if (!hasDigit) missingRequirements.Add("digit");
        if (!hasSpecial) missingRequirements.Add("special character");

        if (missingRequirements.Count > 0)
        {
            errorMessage = $"Password must contain at least one: {string.Join(", ", missingRequirements)}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Gets a description of password requirements.
    /// </summary>
    /// <returns>A description string.</returns>
    public static string GetRequirementsDescription()
    {
        return $"Password must be at least {MinimumLength} characters and contain: " +
               "uppercase letter, lowercase letter, digit, and special character.";
    }
}
