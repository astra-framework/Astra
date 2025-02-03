using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Tooltip
{
    public static void Normal(string text, in TooltipStyle style)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, style.BorderThickness);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, style.BackgroundColor.ToVector4());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToVector4());
        ImGui.PushFont(style.Font.GetImFont());
        ImGui.SetItemTooltip(text);
        ImGui.PopFont();
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(3);
    }

    public static void Begin(in TooltipStyle style)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, style.BorderThickness);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, style.BackgroundColor.ToVector4());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToVector4());
        ImGui.PushFont(style.Font.GetImFont());
        ImGui.BeginTooltip();
    }

    public static void End()
    {
        ImGui.EndTooltip();
        ImGui.PopFont();
        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(3);
    }
}