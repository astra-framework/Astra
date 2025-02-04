using System.Numerics;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class ComboBox
{
    public static void Normal(string id, string preview, in ComboBoxStyle style, Action content, float width = 0)
    {
        ImGuiWindow* window = ImGuiP.GetCurrentWindow();
        if (width == 0)
        {
            width = window->Size.X - window->WindowPadding.X * 2;
        }
        ImGui.PushFont(style.Font.GetImFont());
        ImGui.PushStyleColor(ImGuiCol.FrameBg, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Button, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, style.BackgroundColor.ToUint32());
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, style.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, style.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, style.Padding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 3));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 3));
        ImGui.SetNextItemWidth(width);
        if (ImGui.BeginCombo($"##{id}", preview, ImGuiComboFlags.None))
        {
            content.Invoke();
            ImGui.EndCombo();
        }
        ImGui.PopStyleColor(6);
        ImGui.PopStyleVar(7);
        ImGui.PopFont();
    }
}