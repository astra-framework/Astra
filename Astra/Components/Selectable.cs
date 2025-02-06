using System.Drawing;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Selectable
{
    public static bool Combo(string label, ref bool selected, in SelectableComboStyle style)
    {
        ImGui.PushFont(style.Font.GetImFont());
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, style.BackgroundActiveColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Header, style.BackgroundActiveColor.ToUint32());
        bool result = ImGui.Selectable(label, ref selected);
        ImGui.PopStyleColor(2);
        ImGui.PopFont();
        return result;
    }

    public static bool Combo(string label, bool selected, in SelectableComboStyle style)
    {
        ImGui.PushFont(style.Font.GetImFont());
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, style.BackgroundActiveColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Header, style.BackgroundActiveColor.ToUint32());
        bool result = ImGui.Selectable(label, selected);
        ImGui.PopStyleColor(2);
        ImGui.PopFont();
        return result;
    }
}