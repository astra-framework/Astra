using System.Drawing;
using Astra.Data;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Components;

public static unsafe class Text
{
    public static void Normal(ReadOnlySpan<byte> fmt, Font font, Color color = default)
    {
        ImGui.PushFont(font.GetImFont());
        if (color != default) ImGui.PushStyleColor(ImGuiCol.Text, color.ToVector4());
        ImGui.Text(fmt);
        if (color != default) ImGui.PopStyleColor();
        ImGui.PopFont();
    }

    public static void Normal(string fmt, Font font, Color color = default)
    {
        ImGui.PushFont(font.GetImFont());
        if (color != default) ImGui.PushStyleColor(ImGuiCol.Text, color.ToVector4());
        ImGui.Text(fmt);
        if (color != default) ImGui.PopStyleColor();
        ImGui.PopFont();
    }
}