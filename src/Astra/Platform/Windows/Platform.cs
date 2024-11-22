using System.Drawing;
using Astra.Data;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Astra.Platform.Windows;

internal static unsafe class Platform
{
    private const ushort windows_11_min_build = 22000;

    /// <summary>
    /// Get the monitor size
    /// </summary>
    /// <param name="width">outputs width of screen</param>
    /// <param name="height">outputs height of screen</param>
    internal static void GetScreenSize(out int width, out int height)
    {
        width = GetSystemMetrics(SM.SM_CXSCREEN);
        height = GetSystemMetrics(SM.SM_CYSCREEN);
    }

    /// <summary>
    /// Check if the current Windows OS is version 11
    /// </summary>
    private static bool isWindows11 => Environment.OSVersion.Version.Build >= windows_11_min_build;

    /// <summary>
    /// Sets the window style based on the current Windows version
    /// </summary>
    internal static void SetWindowStyle(in HWND hWnd, in WindowOptions options)
    {
        // Check if the current Windows version is 11, Windows 10 doesn't require any changes
        if (!isWindows11) return;
        // Set window style to rounded corners
        uint val = (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &val, sizeof(uint));

        if (options.NativeBorder == false)
        {
            // Remove window border
            uint val2 = 0xFFFFFFFE;
            DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &val2, sizeof(uint));
        }
    }

    /// <summary>
    /// Get the position of the window
    /// </summary>
    /// <param name="hWnd">handle of window</param>
    /// <returns>Position of the window</returns>
    internal static Point GetWindowPosition(in HWND hWnd)
    {
        RECT rect;
        GetWindowRect(hWnd, &rect);
        return new Point(rect.left, rect.top);
    }
}