using System.Drawing;

namespace Astra.Types.Interfaces;

public interface IBackend
{
    public void Setup(IWindow window);

    public void Destroy();

    public void Poll();

    public void OnResize(uint width, uint height);

    public Size GetViewportSize();

    public Action OnRender { get; set; }
}