using System.Drawing;
using System.Numerics;
using Astra.Extensions;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Checkbox
{
    private static readonly Dictionary<uint, float> fade_timers = [];

    public static bool Normal(string label, ref bool value, in CheckboxStyle style, bool disabled = false)
    {
        ImGuiWindow* window = ImGuiP.GetCurrentWindow();
        if (window->SkipItems == 1) return false;

        ImGuiIO* io = ImGui.GetIO();
        Vector2 pos = ImGui.GetCursorScreenPos();
        uint uId = ImGui.GetID(label);
        bool noLabel = label.StartsWith("##");

        ImGui.PushFont(style.Font.GetImFont());
        Vector2 labelSize = noLabel ? Vector2.Zero : ImGui.CalcTextSize(label);

        ImRect rect = new ImRect(pos, pos + new Vector2(style.Size + style.Size));
        ImRect clickBb = new ImRect(rect.Min, rect.Max + new Vector2(style.Size + ImGui.CalcTextSize(label).X, 0));

        ImGuiP.ItemSize(rect, 0);
        if (ImGuiP.ItemAdd(rect, uId) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiP.ButtonBehavior(clickBb, uId, &hovered, &held);

        if (pressed)
        {
            value = !value;
            fade_timers[uId] = 0;
        }

        ImDrawList* drawList = ImGui.GetWindowDrawList();

        Color backgroundColor;
        Color borderColor;
        Color checkmarkColor;
        Color textColor;

        if (disabled)
        {
            backgroundColor = style.BackgroundDisabledColor;
            borderColor = style.BorderDisabledColor;
            checkmarkColor = style.CheckmarkDisabledColor;
            textColor = style.TextDisabledColor;
        }
        else
        {
            if (hovered || value)
            {
                fade_timers.TryAdd(uId, 0);
                fade_timers[uId] += io->DeltaTime * style.FadeinSpeed / 100f;
                fade_timers[uId] = Math.Min(fade_timers[uId], 1);
            }
            else
            {
                if (fade_timers.ContainsKey(uId))
                {
                    fade_timers[uId] -= io->DeltaTime * style.FadeoutSpeed / 100f;
                    fade_timers[uId] = Math.Max(fade_timers[uId], 0);
                }
            }

            if (value)
            {
                float lerpAmount = fade_timers.GetValueOrDefault(uId, 0f);
                backgroundColor = ColorExtensions.Lerp(style.BackgroundHoverColor, style.BackgroundActiveColor, lerpAmount);
                borderColor = ColorExtensions.Lerp(style.BorderHoverColor, style.BorderActiveColor, lerpAmount);
                checkmarkColor = ColorExtensions.Lerp(style.BackgroundHoverColor, style.CheckmarkColor, lerpAmount);
                textColor = ColorExtensions.Lerp(style.TextHoverColor, style.TextActiveColor, lerpAmount);
            }
            else if (held == false)
            {
                float lerpAmount = fade_timers.GetValueOrDefault(uId, 0f);
                backgroundColor = ColorExtensions.Lerp(style.BackgroundColor, style.BackgroundHoverColor, lerpAmount);
                borderColor = ColorExtensions.Lerp(style.BorderColor, style.BorderHoverColor, lerpAmount);
                checkmarkColor = ColorExtensions.Lerp(style.CheckmarkColor, style.CheckmarkHoverColor, lerpAmount);
                textColor = ColorExtensions.Lerp(style.TextColor, style.TextHoverColor, lerpAmount);
            }
            else
            {
                backgroundColor = style.BackgroundColor;
                borderColor = style.BorderColor;
                checkmarkColor = style.CheckmarkColor;
                textColor = style.TextColor;
            }
        }

        drawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32(), style.Radius);
        if (style.BorderThickness > 0)
        {
            drawList->AddRect(rect.Min, rect.Max, borderColor.ToUint32(), style.Radius, style.BorderThickness);
        }
        if (value)
        {
            Vector2 checkmarkCenter = rect.Min + new Vector2(style.Size / 2, style.Size / 2);
            ImGuiP.RenderCheckMark(drawList, checkmarkCenter,  checkmarkColor.ToUint32(), style.Size);
        }

        if (noLabel == false)
        {
            Vector2 textPos = new Vector2(rect.Max.X + 8, rect.Min.Y + (rect.GetHeight() - labelSize.Y) / 2);
            drawList->AddText(style.Font.GetImFont(), style.Font.Size, textPos, textColor.ToUint32(), label);
        }

        ImGui.PopFont();
        return pressed;
    }
}