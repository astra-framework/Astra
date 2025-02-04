using System.Drawing;
using Astra.Data;

namespace Astra.Styles;

public struct ModalStyle
{
    public Font TitlebarFont;

    public Color TitlebarBackgroundColor;
    public Color TitlebarBorderColor;

    public Color BackgroundDimColor;
    public Color BackgroundColor;
    public Color BorderColor;

    public float Rounding;
    public float BorderThickness;
    public float TitlebarBorderThickness;
    public float TitlebarHeight;

    public bool NoTitlebar;
}