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

        ImGui.PushStyleColor(ImGuiCol.Text, style.TextColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToUint32());

        /* Combobox */
        ImGui.PushStyleColor(ImGuiCol.FrameBg, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, style.BackgroundHoverColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.Button, style.BackgroundColor.ToUint32());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, style.BackgroundHoverColor.ToUint32());

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, style.Padding);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, style.BorderThickness);

        /* Dropdown */
        ImGui.PushStyleColor(ImGuiCol.PopupBg, style.DropdownBackgroundColor.ToUint32());

        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, style.DropdownRadius);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, style.DropdownBorderSize);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, style.Padding); // 0
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,style.Padding); // 0

        ImGui.SetNextItemWidth(width);
        if (ImGui.BeginCombo($"##{id}", preview, ImGuiComboFlags.None))
        {
            content.Invoke();
            ImGui.EndCombo();
        }
        ImGui.PopStyleVar(7);
        ImGui.PopStyleColor(7);
        ImGui.PopFont();
    }
}