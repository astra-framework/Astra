using System.Numerics;
using Astra.Styles;
using Astra.Types.Enums;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Panel
{
    public static void Begin(string id, in PanelStyle style, Vector2 size = default)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, style.BackgroundColor.ToVector4());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToVector4());
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, style.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding);
        ImGuiWindow* parentWindow = ImGuiP.GetCurrentWindow();
        if (style.Display == Display.Flex)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
        }
        else if (style.Display == Display.Fill)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
            size.Y = parentWindow->Size.Y - parentWindow->WindowPadding.Y * 2;
        }
        ImGui.BeginChild(id, size, ImGuiChildFlags.Borders | ImGuiChildFlags.AlwaysUseWindowPadding);
    }

    public static void End()
    {
        ImGui.EndChild();
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}