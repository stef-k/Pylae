namespace Pylae.Core.Security;

/// <summary>
/// Validates QuickCode format and requirements.
/// </summary>
public static class QuickCodeValidator
{
    private const int MinimumLength = 4;
    private const int MaximumLength = 6;

    /// <summary>
    /// Validates that a QuickCode meets format requirements (4-6 digits).
    /// </summary>
    /// <param name="quickCode">The QuickCode to validate.</param>
    /// <param name="errorMessage">The error message if validation fails.</param>
    /// <returns>True if QuickCode is valid, false otherwise.</returns>
    public static bool IsValid(string? quickCode, out string errorMessage)
    {
        if (string.IsNullOrEmpty(quickCode))
        {
            errorMessage = "QuickCode cannot be empty.";
            return false;
        }

        if (quickCode.Length < MinimumLength || quickCode.Length > MaximumLength)
        {
            errorMessage = $"QuickCode must be between {MinimumLength} and {MaximumLength} digits.";
            return false;
        }

        if (!quickCode.All(char.IsDigit))
        {
            errorMessage = "QuickCode must contain only digits (0-9).";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Gets a description of QuickCode requirements.
    /// </summary>
    /// <returns>A description string.</returns>
    public static string GetRequirementsDescription()
    {
        return $"QuickCode must be {MinimumLength}-{MaximumLength} digits (0-9 only).";
    }
}
