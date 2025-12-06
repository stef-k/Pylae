using System.Reflection;
using System.Windows.Forms;

namespace Pylae.Desktop.Helpers;

/// <summary>
/// Extension methods for DataGridView performance optimization.
/// </summary>
public static class DataGridViewExtensions
{
    /// <summary>
    /// Enables double buffering on a DataGridView to reduce flicker.
    /// </summary>
    public static void EnableDoubleBuffering(this DataGridView grid)
    {
        // DoubleBuffered is protected, so we use reflection
        var type = grid.GetType();
        var prop = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        prop?.SetValue(grid, true, null);
    }

    /// <summary>
    /// Binds data to a DataGridView with layout suspension for better performance.
    /// </summary>
    public static void BindDataOptimized<T>(this DataGridView grid, IList<T> data)
    {
        grid.SuspendLayout();
        try
        {
            grid.DataSource = null;
            grid.DataSource = new System.ComponentModel.BindingList<T>(data);
        }
        finally
        {
            grid.ResumeLayout();
        }
    }
}
