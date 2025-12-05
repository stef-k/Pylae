using Pylae.Core.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pylae.Desktop.Services;

/// <summary>
/// Validates photo files for member photos.
/// </summary>
public class PhotoValidator : IPhotoValidator
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private const int MaxWidth = 2000;
    private const int MaxHeight = 2000;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png"
    };

    private static readonly HashSet<ImageFormat> AllowedFormats = new()
    {
        ImageFormat.Jpeg,
        ImageFormat.Png
    };

    public bool ValidatePhotoFile(string filePath, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!File.Exists(filePath))
        {
            errorMessage = "Photo file does not exist.";
            return false;
        }

        var fileInfo = new FileInfo(filePath);

        // Check file size
        if (fileInfo.Length > MaxFileSizeBytes)
        {
            errorMessage = $"Photo file size exceeds maximum of {MaxFileSizeBytes / 1024 / 1024} MB.";
            return false;
        }

        // Check file extension
        if (!AllowedExtensions.Contains(fileInfo.Extension))
        {
            errorMessage = "Photo must be JPEG or PNG format.";
            return false;
        }

        try
        {
            // Try to load as image and check dimensions
            using var image = Image.FromFile(filePath);

            if (!AllowedFormats.Contains(image.RawFormat))
            {
                errorMessage = "Photo must be JPEG or PNG format.";
                return false;
            }

            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                errorMessage = $"Photo dimensions exceed maximum of {MaxWidth}x{MaxHeight} pixels.";
                return false;
            }

            return true;
        }
        catch (OutOfMemoryException)
        {
            errorMessage = "Invalid or corrupted image file.";
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to validate photo: {ex.Message}";
            return false;
        }
    }

    public bool ValidatePhotoData(byte[] photoData, string fileName, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (photoData == null || photoData.Length == 0)
        {
            errorMessage = "Photo data is empty.";
            return false;
        }

        // Check file size
        if (photoData.Length > MaxFileSizeBytes)
        {
            errorMessage = $"Photo file size exceeds maximum of {MaxFileSizeBytes / 1024 / 1024} MB.";
            return false;
        }

        // Check file extension
        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension))
        {
            errorMessage = "Photo must be JPEG or PNG format.";
            return false;
        }

        try
        {
            // Try to load as image from bytes
            using var ms = new MemoryStream(photoData);
            using var image = Image.FromStream(ms);

            if (!AllowedFormats.Contains(image.RawFormat))
            {
                errorMessage = "Photo must be JPEG or PNG format.";
                return false;
            }

            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                errorMessage = $"Photo dimensions exceed maximum of {MaxWidth}x{MaxHeight} pixels.";
                return false;
            }

            return true;
        }
        catch (OutOfMemoryException)
        {
            errorMessage = "Invalid or corrupted image file.";
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to validate photo: {ex.Message}";
            return false;
        }
    }
}
