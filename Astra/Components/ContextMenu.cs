using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static class ContextMenu
{
    public static void Normal(string name, in ContextMenuStyle style, Action content)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, style.BorderThickness);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToUint32());
        if (ImGui.BeginPopupContextItem(name))
        {
            content.Invoke();
            ImGui.EndPopup();
        }
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(3);
    }
}