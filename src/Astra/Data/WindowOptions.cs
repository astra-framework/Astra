using System.Drawing;
using Astra.Types.Enums;

namespace Astra.Data;

public readonly struct WindowOptions()
{
    public readonly string Title { get; init; }

    public readonly Size Size { get; init; }

    public readonly Size MinSize { get; init; } = new(0, 0);

    public readonly Point StartPosition { get; init; } = new(-1, -1);

    public readonly bool Transparent { get; init; } = false;

    public readonly bool Resizable { get; init; } = true;

    public readonly bool NoTitleBar { get; init; } = false;
    public readonly bool NativeBorder { get; init; } = true;

    public readonly BackendApi BackendApi { get; init; } = BackendApi.Auto;
}