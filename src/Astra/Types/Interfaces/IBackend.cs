using System.Drawing;

namespace Astra.Types.Interfaces;

public interface IBackend
{
    /// <summary>
    /// Set up the backend for the given window context
    /// </summary>
    /// <param name="window"></param>
    internal void Setup(in IWindow window);

    internal void Destroy();

    internal void Render();

    internal void OnResize(uint width, uint height);

    public Size GetViewportSize();
}