using System.Windows.Forms;
using Microsoft.Win32;

namespace Pylae.Desktop.Services;

/// <summary>
/// Keeps WinForms synced with the system light/dark/high-contrast theme using the native framework support in .NET 10.
/// </summary>
public sealed class ThemeService : IDisposable
{
    private bool _disposed;

    public ThemeService()
    {
        ApplySystemTheme();
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    public void ApplySystemTheme()
    {
        Application.SetColorMode(SystemColorMode.System);
    }

    private void OnUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category is UserPreferenceCategory.General or UserPreferenceCategory.VisualStyle)
        {
            ApplySystemTheme();
            foreach (Form form in Application.OpenForms)
            {
                form.Invalidate(true);
                form.Refresh();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        _disposed = true;
    }
}
