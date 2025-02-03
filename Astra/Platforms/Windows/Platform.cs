using System.Numerics;
using Astra.Data;
using Hexa.NET.ImGui;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace Astra.Platforms.Windows;

internal static unsafe class Platform
{
    internal const string MINIMIZE_ICON = "\uE921";
    internal const string MAXIMIZE_ICON = "\uE922";
    internal const string RESTORE_ICON = "\uE923";
    internal const string CLOSE_ICON = "\uE8BB";
    internal static readonly Vector2 MAXIMIZED_PADDING = new(8, 8);

    private const ushort windows_11_min_build = 22000;
    private const string segoe_icons_font = @"C:\Windows\Fonts\SegoeIcons.ttf";
    private const string segmdl12_font = @"C:\Windows\Fonts\segmdl2.ttf";

    internal static Font SystemFont { get; private set; } = null!;

    internal static bool IsWindows11 => Environment.OSVersion.Version.Build >= windows_11_min_build;

    /// <summary>
    /// Get current screen size
    /// </summary>
    /// <param name="width">screenWidth</param>
    /// <param name="height">screenHeight</param>
    internal static void GetScreenSize(out int width, out int height)
    {
        width = GetSystemMetrics(SM.SM_CXSCREEN);
        height = GetSystemMetrics(SM.SM_CYSCREEN);
    }

    /// <summary>
    /// Set window style based of windows version
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="options"></param>
    internal static void SetWindowStyle(in HWND hWnd, in WindowOptions options)
    {
        // Check if the current Windows version is 11, Windows 10 doesn't require any changes
        if (!IsWindows11) return;
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

    internal static void LoadSystemFont()
    {
        ImGuiIO* io = ImGui.GetIO();
        // If "Windows11" System font is available use it, otherwise use the Windows 10 font
        if (File.Exists(segoe_icons_font))
        {
            uint[] range = ['\uE001', '\uF8CC', '\0'];
            SystemFont = new Font(segoe_icons_font, 10, range);
            fixed(uint* pRange = range)
                SystemFont.ImFont = io->Fonts->AddFontFromFileTTF(SystemFont.Path, SystemFont.Size, null, pRange);
        }
        else if (File.Exists(segmdl12_font))
        {
            uint[] range = ['\uE001', '\uF8B3', '\0'];
            SystemFont = new Font(segmdl12_font, 10, range);
            fixed(uint* pRange = range)
                SystemFont.ImFont = io->Fonts->AddFontFromFileTTF(SystemFont.Path, SystemFont.Size, null, pRange);
        }
        else
        {
            // for now, we just "fallback" to the default font if both fonts are not available
            SystemFont = new Font(string.Empty, 10)
            {
                ImFont = ImGui.GetFont()
            };
        }
    }
}