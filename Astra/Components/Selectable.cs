using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public class Selectable
{
    public static bool Combo(string label, ref bool selected, in SelectableComboStyle style)
    {
        ImGui.PushStyleColor(ImGuiCol.Header, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, style.BackgroundHoverColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.HeaderActive, style.BackgroundActiveColor.ToUint32());
        bool result = ImGui.Selectable(label, ref selected);
        ImGui.PopStyleColor(3);
        return result;
    }
}