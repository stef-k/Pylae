namespace Pylae.Core.Interfaces;

/// <summary>
/// Validates photo files for member photos.
/// </summary>
public interface IPhotoValidator
{
    /// <summary>
    /// Validates a photo file for type, size, and dimensions.
    /// </summary>
    /// <param name="filePath">Path to the photo file.</param>
    /// <param name="errorMessage">Error message if validation fails.</param>
    /// <returns>True if photo is valid, false otherwise.</returns>
    bool ValidatePhotoFile(string filePath, out string errorMessage);

    /// <summary>
    /// Validates photo data (bytes) for type and size.
    /// </summary>
    /// <param name="photoData">Photo file data.</param>
    /// <param name="fileName">Original file name (for extension checking).</param>
    /// <param name="errorMessage">Error message if validation fails.</param>
    /// <returns>True if photo is valid, false otherwise.</returns>
    bool ValidatePhotoData(byte[] photoData, string fileName, out string errorMessage);
}
