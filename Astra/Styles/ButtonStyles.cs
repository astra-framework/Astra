using System.Drawing;
using Astra.Data;
using Astra.Types.Enums;

namespace Astra.Styles;

public struct ButtonStyle
{
    public Padding Padding;
    public Font Font;
    public TextAlign TextAlign;
    public Display Display;

    public uint BorderThickness;
    public uint Radius;
    public uint FadeinSpeed;
    public uint FadeoutSpeed;

    public Color BackgroundColor;
    public Color BackgroundHoverColor;
    public Color BackgroundActiveColor;
    public Color BackgroundDisabledColor;

    public Color TextColor;
    public Color TextHoverColor;
    public Color TextActiveColor;
    public Color TextDisabledColor;

    public Color BorderColor;
    public Color BorderHoverColor;
    public Color BorderActiveColor;
    public Color BorderDisabledColor;
}

public struct IconButtonStyle
{
    public Padding Padding;
    public Font Font;
    public Display Display;

    public uint BorderThickness;
    public uint Radius;
    public uint FadeinSpeed;
    public uint FadeoutSpeed;

    public Color BackgroundColor;
    public Color BackgroundHoverColor;
    public Color BackgroundActiveColor;
    public Color BackgroundDisabledColor;

    public Color TextColor;
    public Color TextHoverColor;
    public Color TextActiveColor;
    public Color TextDisabledColor;

    public Color BorderColor;
    public Color BorderHoverColor;
    public Color BorderActiveColor;
    public Color BorderDisabledColor;
}