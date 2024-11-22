using Astra.Data;

namespace Astra.Types.Interfaces;

public interface IWindow
{
    public IBackend Backend { get; set; }

    public void Setup(in WindowOptions windowOptions);

    public void Destroy();

    public void PollEvents();

    public nint GetHandle();
}