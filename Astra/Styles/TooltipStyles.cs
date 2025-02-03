using System.Drawing;
using System.Numerics;
using Astra.Data;

namespace Astra.Styles;

public struct TooltipStyle
{
    public Vector2 Padding;
    public Font Font;

    public uint BorderThickness;
    public uint Radius;

    public Color BackgroundColor;
    public Color BorderColor;
}