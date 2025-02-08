using System.Drawing;
using Astra.Data;

namespace Astra.Styles;

public struct CheckboxStyle
{
    public Font Font;

    public float Size;
    public float BorderThickness;
    public float Radius;

    public float FadeinSpeed;
    public float FadeoutSpeed;

    public Color BackgroundColor;
    public Color BackgroundHoverColor;
    public Color BackgroundActiveColor;
    public Color BackgroundDisabledColor;

    public Color CheckmarkColor;
    public Color CheckmarkHoverColor;
    public Color CheckmarkDisabledColor;

    public Color BorderColor;
    public Color BorderHoverColor;
    public Color BorderActiveColor;
    public Color BorderDisabledColor;

    public Color TextColor;
    public Color TextHoverColor;
    public Color TextActiveColor;
    public Color TextDisabledColor;
}