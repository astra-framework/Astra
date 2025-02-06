using System.Drawing;
using System.Numerics;
using Astra.Data;

namespace Astra.Styles;

public struct ComboBoxStyle
{
    public Font Font;
    public Vector2 Padding;

    public Color TextColor;
    public Color BorderColor;

    public Color BackgroundColor;
    public Color BackgroundHoverColor;

    public float BorderThickness;
    public float Radius;

    public Color DropdownBackgroundColor;

    public float DropdownRadius;
    public float DropdownBorderSize;
}