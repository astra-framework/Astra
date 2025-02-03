namespace Astra.Data;

public struct Padding(int left, int top, int right, int bottom)
{
    public int Left = left;
    public int Top = top;
    public int Right = right;
    public int Bottom = bottom;

    public Padding(int all) : this(all, all, all, all) {}
}