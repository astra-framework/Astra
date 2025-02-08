using System.Drawing;
using System.Numerics;
using Astra.Managers;
using Hexa.NET.ImGui;

namespace Astra.Util;

public static unsafe class ImUtils
{
    internal static ImGuiContext* SetupImGui()
    {
        ImGuiContext* context = ImGui.CreateContext();
        ImGuiIO* io = ImGui.GetIO();
        io->IniFilename = null;
        io->WantSaveIniSettings = 0;

        ImGuiStyle* style = ImGui.GetStyle();
        style->WindowRounding = 0;
        style->WindowBorderSize = 0;
        style->WindowPadding = new Vector2(0, 0);
        style->FramePadding = new Vector2(0, 0);
        style->ItemSpacing = new Vector2(0, 0);

        if (OperatingSystem.IsWindows())
        {
            Platforms.Windows.Platform.LoadSystemFont();
        }
        FontManager.LoadFonts();

        return context;
    }

    public static Vector4 ToVector4(this Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public static Vector4 ToVector4(this Color color, byte alpha)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, alpha / 255f);
    }

    public static uint ToUint32(this Color color)
    {
        return ImGui.GetColorU32(color.ToVector4());
    }

    public static uint ToUint32(this Color color, byte alpha)
    {
        return ImGui.GetColorU32(color.ToVector4(alpha));
    }

    public static Vector2 GetCenter(this ImRect rect)
    {
        return new Vector2((rect.Min.X + rect.Max.X) / 2, (rect.Min.Y + rect.Max.Y) / 2);
    }


    public static float GetHeight(this ImRect rect)
    {
        return rect.Max.Y - rect.Min.Y;
    }
}