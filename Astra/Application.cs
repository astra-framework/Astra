using Astra.Components.Internal;
using Astra.Data;
using Astra.Styles;
using Astra.Types.Interfaces;

namespace Astra;

public static class Application
{
    public static IWindow CreateWindow(WindowOptions options, TitlebarStyle titlebarStyle, Action onRender, Action? titlebarContentRender = null)
    {
        Titlebar.SetStyle(titlebarStyle);
        Titlebar.SetContentRender(titlebarContentRender);
        IWindow window;
        if (OperatingSystem.IsWindows())
        {
            window = new Platforms.Windows.Window();
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported platform");
        }
        window.Setup(options, onRender);
        return window;
    }
}