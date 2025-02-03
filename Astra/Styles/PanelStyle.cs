using System.Drawing;
using System.Numerics;
using Astra.Types.Enums;

namespace Astra.Styles;

public struct PanelStyle
{
    public Vector2 Padding { get; set; }
    public Display Display { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BorderColor { get; set; }

    public uint BorderThickness;
    public uint Radius;
}