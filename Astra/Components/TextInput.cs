using System.Drawing;
using System.Numerics;
using Astra.Extensions;
using Astra.Styles;
using Astra.Types.Enums;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class TextInput
{
    private static readonly Dictionary<uint, float> fade_timers = [];
    private static readonly List<uint> active_states = [];

    public static void Normal(string id, ref string text, uint maxLength, in TextInputStyle style, bool disabled = false, Vector2 size = default)
    {
        ImGuiWindow* parentWindow = ImGuiP.GetCurrentWindow();
        ImGuiIO* io = ImGui.GetIO();

        size.Y = style.Height;
        if (style.Display == Display.Flex)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
        }
        else if (style.Display == Display.Fill)
        {
            size.X = parentWindow->Size.X - parentWindow->WindowPadding.X * 2;
            size.Y = parentWindow->Size.Y - parentWindow->WindowPadding.Y * 2;
        }

        ImRect rect = new ImRect(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + size);
        bool hovered = ImGui.IsMouseHoveringRect(rect.Min, rect.Max);
        uint uId = ImGui.GetID(id);

        bool active = active_states.Contains(uId);

        Color backgroundColor;
        Color textColor;
        Color borderColor;

        if (disabled)
        {
            backgroundColor = style.BackgroundDisabledColor;
            textColor = style.TextDisabledColor;
            borderColor = style.BorderDisabledColor;
        }
        else
        {
            if (hovered)
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

            if (active == false)
            {
                float lerpAmount = fade_timers.GetValueOrDefault(uId, 0f);
                backgroundColor = ColorExtensions.Lerp(style.BackgroundColor, style.BackgroundHoverColor, lerpAmount);
                textColor = ColorExtensions.Lerp(style.TextColor, style.TextHoverColor, lerpAmount);
                borderColor = ColorExtensions.Lerp(style.BorderColor, style.BorderHoverColor, lerpAmount);
            }
            else
            {
                backgroundColor = style.BackgroundActiveColor;
                textColor = style.TextActiveColor;
                borderColor = style.BorderActiveColor;
            }
        }

        ImGui.PushStyleColor(ImGuiCol.ChildBg, backgroundColor.ToVector4());
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4());
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, style.Radius);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, style.BorderThickness);

        Vector2 padding = style.Padding;
        padding.Y = 0;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding);
        ImGui.BeginChild($"{id}_child", size, ImGuiChildFlags.Borders | ImGuiChildFlags.AlwaysUseWindowPadding);
        {
            ImGui.PushFont(style.Font.GetImFont());

            ImGui.PushStyleColor(ImGuiCol.FrameBg, Color.Transparent.ToVector4());
            ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4());
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, padding with { Y = (size.Y - style.Font.GetImFont()->FontSize) / 2 });
            ImGui.SetNextItemWidth(size.X - padding.X * 2);
            ImGui.InputText($"##{id}_input", ref text, maxLength);
            if (ImGui.IsItemActive())
            {
                if (active_states.Contains(uId) == false)
                {
                    active_states.Add(uId);
                }
            }
            else
            {
                if (active_states.Contains(uId))
                {
                    active_states.Remove(uId);
                }
            }
            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(1);

            ImGui.PopFont();
        }
        ImGui.EndChild();
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(2);
    }
}