using System.Drawing;
using Astra.Types.Enums;

namespace Astra.Data;

public readonly struct WindowOptions
{
    /// <summary>
    /// Window Title
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Window class name
    /// </summary>
    public string ClassName { get; init; }


    /// <summary>
    /// Window start size
    /// </summary>
    public Size Size { get; init; }

    /// <summary>
    /// Window min size
    /// </summary>
    public Size MinSize { get; init; }

    /// <summary>
    /// Window start position (setting this to -1, -1 will make window start center of screen)
    /// </summary>
    public Point StartPosition { get; init; }


    /// <summary>
    /// If set to ture, end-user is able to resize the window
    /// </summary>
    public bool Resizable { get; init; }

    /// <summary>
    /// If set to true, window will have native border
    /// </summary>
    public bool NativeBorder { get; init; }


    /// <summary>
    /// BackendApi to use for rendering
    /// </summary>
    public BackendApi BackendApi { get; init; }


    public static readonly WindowOptions DEFAULT = new()
    {
        Title = "Astra Window",
        ClassName = "astra",
        Size = new Size(800, 500),
        MinSize = new Size(300, 100),
        StartPosition = new Point(-1, -1),
        Resizable = true,
        NativeBorder = true,
        BackendApi = BackendApi.Auto
    };
}