using System.Drawing;
using System.Runtime.InteropServices;
using Astra.Backends;
using Astra.Components.Internal;
using Astra.Data;
using Astra.Types.Interfaces;
using Hexa.NET.ImGui.Backends.Win32;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;


namespace Astra.Platforms.Windows;

public unsafe class Window : IWindow
{
    public IBackend Backend { get; set; } = null!;

    private const byte loop_timer_id = 1;
    private const byte border_width = 8;

    private HWND handle;
    private WNDCLASSEXW wndClass;
    private WindowOptions options;

    private bool running = true;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate managedWndProc = null!;

    public void Setup(in WindowOptions windowOptions, Action onRender)
    {
        options = windowOptions;
        managedWndProc = wndProc;

        // When more backends are added, this will be changed to a switch statement
        Backend = new Direct3D11();

        Backend.OnRender = onRender;

        int x = options.StartPosition.X;
        int y = options.StartPosition.Y;

        Platform.GetScreenSize(out int monitorWidth, out int monitorHeight);

        // If start position is set to -1, -1, center the window
        if (x == -1 && y == -1)
        {
            x = (monitorWidth - options.Size.Width) / 2;
            y = (monitorHeight - options.Size.Height) / 2;
        }

        // Check if the window is out of bounds and reset to 0 if so
        if (x > monitorWidth || x < 0) x = 0;
        if (y > monitorHeight || y < 0) y = 0;

        fixed (char* pClassName = windowOptions.ClassName)
        fixed (char* pTitle = windowOptions.Title)
        {
            var lWndClass = new WNDCLASSEXW
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = CS.CS_CLASSDC,
                lpfnWndProc = (delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(managedWndProc),
                hInstance = GetModuleHandleW(null),
                hCursor = HCURSOR.NULL,
                hbrBackground = HBRUSH.NULL,
                lpszClassName = pClassName
            };

            RegisterClassExW(&lWndClass);
            wndClass = lWndClass;

            handle = CreateWindowExW(0, pClassName, pTitle, WS.WS_OVERLAPPEDWINDOW, x, y, options.Size.Width, options.Size.Height, HWND.NULL, HMENU.NULL, wndClass.hInstance, null);
        }

        Backend.Setup(this);
        Platform.SetWindowStyle(handle, options);

        ShowWindow(handle, SW.SW_SHOW);
        UpdateWindow(handle);
    }


    public void Destroy()
    {
        DestroyWindow(handle);
        UnregisterClassW(wndClass.lpszClassName, wndClass.hInstance);
    }


    public void Poll()
    {
        // ReSharper disable once TooWideLocalVariableScope
        MSG msg;
        while (running)
        {
            if (PeekMessageW(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE) != 0)
            {
                TranslateMessage(&msg);
                DispatchMessageW(&msg);
                if (msg.message == WM.WM_QUIT) running = false;
            }
            else
            {
                if (IsIconic(handle))
                {
                    Thread.Sleep(10);
                    continue;
                }
                Backend.Poll();
                if (Titlebar.IsDragging)
                {
                    DragWindow();
                    /*
                     * This is a workaround for the issue where the window keeps being dragged after
                     * releasing the mouse button and pressing down again in the window even if the mouse is not in the titlebar
                     */
                    mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
            }
        }
        Destroy();
    }


    public void DragWindow()
    {
        ReleaseCapture();
        SendMessageW(handle, WM.WM_NCLBUTTONDOWN, HTCAPTION, 0);
    }


    public bool IsMinimized()
    {
        return IsIconic(handle);
    }


    public bool IsMaximized()
    {
        return IsZoomed(handle);
    }


    public bool CanResize()
    {
        return options.Resizable;
    }


    public void Minimize()
    {
        OnMinimize?.Invoke();
        ShowWindow(handle, SW.SW_MINIMIZE);
    }


    public void Maximize()
    {
        OnMaximize?.Invoke();
        ShowWindowAsync(handle, SW.SW_MAXIMIZE);
    }


    public void Restore()
    {
        OnRestore?.Invoke();
        ShowWindowAsync(handle, SW.SW_RESTORE);
    }


    public void Hide()
    {
        ShowWindowAsync(handle, SW.SW_HIDE);
    }


    public void Show()
    {
        ShowWindowAsync(handle, SW.SW_SHOW);
    }


    public void Close()
    {
        OnClose?.Invoke();
        PostMessageW(handle, WM.WM_CLOSE, 0, 0);
    }


    public Size GetSize()
    {
        RECT rect;
        GetClientRect(handle, &rect);
        return new Size(rect.right - rect.left, rect.bottom - rect.top);
    }


    public nint GetHandle()
    {
        return handle;
    }


    public event Action? OnClose;
    public event Action? OnMinimize;
    public event Action? OnMaximize;
    public event Action? OnRestore;
    public event Action<string>? OnFileDrop;


    private LRESULT wndProc(HWND window, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (ImGuiImplWin32.WndProcHandler(window, msg, wParam, lParam) > 0) return 1;
        switch (msg)
        {
            case WM.WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                Backend.OnResize(LOWORD(lParam), HIWORD(lParam));
                Backend.Poll();
                return 0;
            }
            case WM.WM_GETMINMAXINFO:
            {
                MINMAXINFO* minmax = (MINMAXINFO*)lParam;
                minmax->ptMinTrackSize.x = options.MinSize.Width;
                minmax->ptMinTrackSize.y = options.MinSize.Height;
                return 0;
            }
            case WM.WM_NCCALCSIZE:
            {
                return 0;
            }
            case WM.WM_SYSCOMMAND:
            {
                if (wParam == SC.SC_MOVE || wParam == SC.SC_SIZE)
                {
                    nint style = GetWindowLongPtrW(handle, GWL.GWL_STYLE);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style | WS.WS_CAPTION);
                    DefWindowProcW(handle, msg, wParam, lParam);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style);
                    return 0;
                }
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) return 0; // Disable ALT application menu
                break;
            }
            case WM.WM_ENTERSIZEMOVE | WM.WM_ENTERMENULOOP:
            {
                nuint ret = SetTimer(handle, loop_timer_id, USER_TIMER_MINIMUM, null);
                if (ret == 0)
                {
                    throw new Exception("Failed to set timer");
                }
                return 0;
            }
            case WM.WM_EXITMENULOOP:
            {
                KillTimer(handle, loop_timer_id);
                return 0;
            }
            case WM.WM_TIMER:
            {
                if (wParam == loop_timer_id)
                {
                    Backend.Poll();
                    return 0;
                }
                return 0;
            }
            case WM.WM_NCLBUTTONDOWN:
            {
                break;
            }
            case WM.WM_NCHITTEST:
            {
                if (options.Resizable)
                {
                    POINT point = new POINT(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                    RECT rc;
                    GetWindowRect(handle, &rc);
                    if (point.y >= rc.top && point.y < rc.top + border_width) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTTOPLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTTOPRIGHT;
                        }
                        return HTTOP;
                    }

                    if (point.y >= rc.bottom - border_width && point.y < rc.bottom) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTBOTTOMLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTBOTTOMRIGHT;
                        }
                        return HTBOTTOM;
                    }

                    if (point.x >= rc.left && point.x < rc.left + border_width) {
                        return HTLEFT;
                    }
                    if (point.x >= rc.right - border_width && point.x < rc.right) {
                        return HTRIGHT;
                    }
                }
                return HTCLIENT;
            }
            case WM.WM_DROPFILES:
            {
                HDROP hDrop = (HDROP)wParam;
                uint fileCount = DragQueryFileW(hDrop, 0xFFFFFFFF, null, 0);

                for (uint i = 0; i < fileCount; i++)
                {
                    uint fileNameLength = DragQueryFileW(hDrop, i, null, 0) + 1;
                    onFileDropped(hDrop, i, fileNameLength);
                }

                DragFinish(hDrop);
                return 0;
            }
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, msg, wParam, lParam);
    }


    private void onFileDropped(in HDROP hDrop, uint index, uint fileNameLength)
    {
        char* fileName = stackalloc char[(int)fileNameLength];
        DragQueryFileW(hDrop, index, fileName, fileNameLength);
        OnFileDrop?.Invoke(new string(fileName));
    }
}