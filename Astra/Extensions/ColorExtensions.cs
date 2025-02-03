using System.Drawing;

namespace Astra.Extensions;

public static class ColorExtensions
{
    public static Color Lerp(Color color, Color target, float amount)
    {
        byte r = (byte)(color.R + (target.R - color.R) * amount);
        byte g = (byte)(color.G + (target.G - color.G) * amount);
        byte b = (byte)(color.B + (target.B - color.B) * amount);
        byte a = (byte)(color.A + (target.A - color.A) * amount);
        return Color.FromArgb(a, r, g, b);
    }
}