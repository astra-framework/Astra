using System.Numerics;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Checkbox
{
    public static bool Normal(string label, ref bool value, in CheckboxStyle style)
    {
        ImGuiWindow* window = ImGuiP.GetCurrentWindow();
        if (window->SkipItems == 1) return false;

        uint id = ImGui.GetID(label);

        bool noLabel = label.StartsWith("##");

        ImGui.PushFont(style.Font.GetImFont());
        Vector2 labelSize = noLabel ? Vector2.Zero : ImGui.CalcTextSize(label);

        Vector2 pos = ImGui.GetCursorScreenPos();
        ImRect rect = new ImRect(pos, pos + new Vector2(style.Size + style.Size));
        ImRect clickBb = new ImRect(rect.Min, rect.Max + new Vector2(style.Size + ImGui.CalcTextSize(label).X, 0));

        var rect3 = clickBb;
        rect3.Max.Y += 2;
        ImGuiP.ItemSize(rect3, 0);
        if (ImGuiP.ItemAdd(rect, id) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiP.ButtonBehavior(clickBb, id,&hovered, &held);

        if (pressed) value = !value;

        ImDrawList* drawList = ImGui.GetWindowDrawList();

        if (value)
        {
            drawList->AddRectFilled(rect.Min, rect.Max, style.BackgroundColor.ToUint32(), style.Radius);
            //Draw checkmark
            var center = rect.GetCenter();
            ImGuiP.RenderCheckMark(drawList, center, style.CheckColor.ToUint32(), 16f);
            /*float radius = float * 0.17f;
            drawList->AddCircleFilled(center, radius, Color.FromArgb(255, 255, 255).ImColor(), 19);*/
            /*if (noLabel == false)
                drawList->AddText(style.Font, style.Font.Size, (style.Size < 20 ? rect.Min - new Vector2(size - 35f, 0.01f * size) : rect.Min + new Vector2(size + 8f, 0.1f * size)), Color.White.ImColor(), label);*/
        }
        else
        {
            drawList->AddRectFilled(rect.Min, rect.Max, style.BackgroundColor.ToUint32(), style.Radius);
            if (hovered)
            {
                drawList->AddRect(rect.Min, rect.Max, style.BorderHoverColor.ToUint32(), style.Radius, ImDrawFlags.RoundCornersAll, style.BorderThickness);
            }
            else
            {
                drawList->AddRect(rect.Min, rect.Max, style.BorderHoverColor.ToUint32(), style.Radius, ImDrawFlags.RoundCornersAll, style.BorderThickness);
            }
            /*if (noLabel == false)
                drawList->AddText(Style.FontRegular16, 16f, (size < 20 ? rect.Min - new Vector2(size - 35f, 0.01f * size) : rect.Min + new Vector2(size + 8f, 0.1f * size)), Color.White.ImColor(), label);*/
        }

        ImGui.PopFont();
        return pressed;
    }
}