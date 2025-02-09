using System.Drawing;
using Astra.Data;

namespace Astra.Types.Interfaces;

public interface IWindow
{
    public IBackend Backend { get; set; }

    public void Setup(in WindowOptions windowOptions, Action onRender);

    public void Destroy();

    public void Poll();

    public void DragWindow();

    public bool IsMinimized();

    public bool IsMaximized();

    public bool CanResize();

    public void Minimize();

    public void Maximize();

    public void Restore();

    public void Hide();

    public void Show();

    public void Close();

    public Size GetSize();

    public nint GetHandle();


    public event Action OnClose;
    public event Action OnMinimize;
    public event Action OnMaximize;
    public event Action OnRestore;
    public event Action<string> OnFileDrop;
}