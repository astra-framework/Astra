namespace Astra.Data;

public readonly struct Padding(float left, float top, float right,float bottom)
{
    public readonly float Left = left;
    public readonly float Top = top;
    public readonly float Right = right;
    public readonly float Bottom = bottom;

    public Padding(float all) : this(all, all, all, all) {}
}