using System.Drawing;
using System.Numerics;
using Astra.Components.Internal;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Modal
{
    public static void Normal(string name, ref bool show, Vector2 size, ModalStyle style, Action content)
    {
        ImGui.PushStyleColor(ImGuiCol.PopupBg, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.ModalWindowDimBg, style.BackgroundDimColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToUint32());
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, style.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, style.Rounding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.SetNextWindowSize(size);
        if (ImGui.BeginPopupModal(name, ref show, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove))
        {
            if (style.NoTitlebar == false)
            {
                ImGuiWindow* window = ImGuiP.GetCurrentWindow();
                ImDrawList* drawList = ImGui.GetWindowDrawList();
                Vector2 pos = ImGui.GetCursorScreenPos();
                ImRect titlebarRect = new ImRect(pos, pos + new Vector2(window->Size.X, style.TitlebarHeight));
                drawList->AddRectFilled(titlebarRect.Min, titlebarRect.Max, style.TitlebarBackgroundColor.ToUint32(),style.Rounding, ImDrawFlags.RoundCornersTop);
                if (style.TitlebarBorderThickness > 0)
                {
                    drawList->AddLine(titlebarRect.Max with { X = 0 }, titlebarRect.Max, style.TitlebarBorderColor.ToUint32(), style.TitlebarBorderThickness);
                }
                ImGui.SetCursorPosX(window->Size.X - style.TitlebarHeight);
                if (Titlebar.WindowsTitlebarButton("close_caption_btn", Platforms.Windows.Platform.CLOSE_ICON, false, new Vector2(style.TitlebarHeight)))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.PushFont(style.TitlebarFont.GetImFont());
                Vector2 textSize = ImGui.CalcTextSize(name);
                ImGui.PopFont();

                ImGui.SetCursorPos(new Vector2(8, style.TitlebarHeight / 2 - textSize.Y / 2));
                Text.Normal(name, style.TitlebarFont, Color.White);
                ImGui.SetCursorPos(new Vector2(0, style.TitlebarHeight + style.TitlebarBorderThickness));
            }
            content();
            ImGui.EndPopup();
        }
        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar(3);
    }
}