using Hexa.NET.ImGui;

namespace Astra.Data;

public unsafe class Font
{
    public string Path { get; set; }
    public float Size { get; set; }
    internal uint[]? Ranges { get; set; }
    internal ImFontConfig* Config { get; set; }

    internal ImFont* ImFont { get; set; }

    public ImFont* GetImFont()
    {
        return ImFont == null ? ImGui.GetFont() : ImFont;
    }

    public Font(string path, float size)
    {
        Path = path;
        Size = size;
    }

    public Font(string path, float size, uint[] ranges)
    {
        Path = path;
        Size = size;
        Ranges = ranges;
    }

    public Font(string path, float size, ImFontConfig* config)
    {
        Path = path;
        Size = size;
        Config = config;
    }
}