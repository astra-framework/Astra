using System.Drawing;
using System.Numerics;
using Astra.Styles;
using Astra.Types.Interfaces;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components.Internal;

public static unsafe class Titlebar
{
    internal static bool IsDragging;

    private static TitlebarStyle style;
    private static Action? contentRender;

    internal static void WindowsTitlebar(IWindow context)
    {
        ImGuiWindow* window = ImGuiP.GetCurrentWindow();
        if (window->SkipItems == 1) return;

        ImDrawList* drawList = ImGui.GetWindowDrawList();

        Vector2 pos = ImGui.GetCursorScreenPos();
        ImRect titlebarRect = new ImRect(pos, pos + new Vector2(window->Size.X, GetHeight(context)));
        drawList->AddRectFilled(titlebarRect.Min, titlebarRect.Max, style.BackgroundColor.ToUint32());


        ImGui.BeginGroup();
        contentRender?.Invoke();
        ImGui.EndGroup();

        if (style.BorderThickness > 0)
        {
            drawList->AddLine(titlebarRect.Max with { X = 0 }, titlebarRect.Max, style.BorderColor.ToUint32(), style.BorderThickness);
        }

        ImGui.SameLine();

        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y);
        if (windowsTitlebarButton("close_caption_btn", Platforms.Windows.Platform.CLOSE_ICON, false, new Vector2(style.Height)))
        {
            context.Close();
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y * 2 + (OperatingSystem.IsWindows() && context.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING.X : 0));
        if (windowsTitlebarButton("toggle_state_caption_btn", context.IsMaximized() ? Platforms.Windows.Platform.RESTORE_ICON : Platforms.Windows.Platform.MAXIMIZE_ICON, false, new Vector2(style.Height)))
        {
            if (context.IsMaximized())
                context.Restore();
            else
                context.Maximize();
        }
        ImGui.SameLine();
        ImGui.SetCursorPosX(window->Size.X - titlebarRect.Max.Y * 3 + (OperatingSystem.IsWindows() && context.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING.X : 0));
        if (windowsTitlebarButton("minimize_caption_btn", Platforms.Windows.Platform.MINIMIZE_ICON, false, new Vector2(style.Height)))
        {
            context.Minimize();
        }

        uint titlebarId = ImGui.GetID("titlebar");
        ImGui.SetNextItemAllowOverlap();
        ImGuiP.ItemSize(titlebarRect, 0);
        if (ImGuiP.ItemAdd(titlebarRect, titlebarId) == false) return;

        bool hovered, held;
        ImGuiP.ButtonBehavior(titlebarRect, titlebarId, &hovered, &held);

        if (hovered && held && IsDragging == false)
            IsDragging = true;
        else if (held == false)
            IsDragging = false;


    }

    private static bool windowsTitlebarButton(string id, string icon, bool disabled, Vector2 size)
    {
        ImGuiWindow* window = ImGuiP.GetCurrentWindow();
        if (window->SkipItems == 1) return false;

        uint uId = ImGui.GetID(id);
        Vector2 position = ImGui.GetCursorScreenPos();

        ImGui.PushFont(Platforms.Windows.Platform.SystemFont.GetImFont());
        Vector2 labelSize = ImGui.CalcTextSize(icon);

        Vector2 buttonSize = size;

        if (size != default) buttonSize = size;
        ImRect rect = new ImRect(position, position + buttonSize);

        ImGuiP.ItemSize(size, 0);
        if (ImGuiP.ItemAdd(rect, uId) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiP.ButtonBehavior(rect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = hovered = pressed = false;
        }

        Color backgroundColor = Color.Transparent;
        Color iconColor = Color.White;

        if (hovered)
        {
            backgroundColor = icon == Platforms.Windows.Platform.CLOSE_ICON ? Color.FromArgb(235, 64, 52) : Color.FromArgb(10, 255, 255, 255);
        }

        ImDrawList* drawList = ImGui.GetWindowDrawList();

        drawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32(), 0f);
        Vector2 centerIconPos = new Vector2(rect.Min.X + (rect.Max.X - rect.Min.X) / 2 - labelSize.X / 2, rect.Min.Y + (rect.Max.Y - rect.Min.Y) / 2 - labelSize.Y / 2);
        drawList->AddText(centerIconPos, iconColor.ToUint32(), icon);

        ImGui.PopFont();

        return pressed;
    }

    public static void SetContentRender(Action? render)
    {
        contentRender = render;
    }

    public static void SetStyle(TitlebarStyle titlebarStyle)
    {
        style = titlebarStyle;
    }

    internal static float GetBorderThickness()
    {
        return style.BorderThickness;
    }

    internal static float GetHeight(IWindow context)
    {
        float height = style.Height + style.BorderThickness;
        return height;
    }
}