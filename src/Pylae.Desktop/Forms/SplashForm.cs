using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

public partial class SplashForm : Form
{
    public SplashForm()
    {
        InitializeComponent();
        LoadAndApplyImage();
    }

    private const int MaxSize = 450;

    private void LoadAndApplyImage()
    {
        var imagePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "splash.png");
        if (!File.Exists(imagePath)) return;

        using var original = new Bitmap(imagePath);

        // Calculate scale to fit within MaxSize while preserving aspect ratio
        var scale = Math.Min((float)MaxSize / original.Width, (float)MaxSize / original.Height);
        var newWidth = (int)(original.Width * scale);
        var newHeight = (int)(original.Height * scale);

        Bitmap bitmap;
        if (newWidth != original.Width || newHeight != original.Height)
        {
            bitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(original, 0, 0, newWidth, newHeight);
        }
        else
        {
            bitmap = new Bitmap(original);
        }

        using (bitmap)
        {
            SetBitmap(bitmap);
        }
    }

    // Per-pixel alpha transparency support
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
            return cp;
        }
    }

    public void SetBitmap(Bitmap bitmap)
    {
        if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            throw new ArgumentException("Bitmap must be 32bpp with alpha channel.");

        var screenDc = GetDC(IntPtr.Zero);
        var memDc = CreateCompatibleDC(screenDc);
        var hBitmap = IntPtr.Zero;
        var oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            oldBitmap = SelectObject(memDc, hBitmap);

            var size = new Size(bitmap.Width, bitmap.Height);
            var pointSource = new Point(0, 0);

            // Center on screen
            var screen = Screen.PrimaryScreen?.WorkingArea ?? Screen.GetBounds(Point.Empty);
            var topPos = new Point(
                (screen.Width - size.Width) / 2 + screen.Left,
                (screen.Height - size.Height) / 2 + screen.Top);

            var blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = AC_SRC_ALPHA
            };

            // Set form size to match image
            Size = size;

            UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA);
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, screenDc);
            if (hBitmap != IntPtr.Zero)
            {
                SelectObject(memDc, oldBitmap);
                DeleteObject(hBitmap);
            }
            DeleteDC(memDc);
        }
    }

    // P/Invoke declarations
    private const int ULW_ALPHA = 0x00000002;
    private const byte AC_SRC_OVER = 0x00;
    private const byte AC_SRC_ALPHA = 0x01;

    [StructLayout(LayoutKind.Sequential)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
        IntPtr hdcSrc, ref Point pptSrc, uint crKey, ref BLENDFUNCTION pblend, uint dwFlags);
}
