using System.Runtime.InteropServices;
using Astra.Backends;
using Astra.Data;
using Astra.Types.Enums;
using Astra.Types.Interfaces;
using Astra.Util;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416 Disables warning -> "CA1416: Validate platform compatibility"

namespace Astra.Platform.Windows;

public unsafe partial class Window : IWindow
{
    #region Additional Imports
    private const string library = "Mochi.DearImGui.Native.dll";

    // ReSharper disable once InconsistentNaming
    [LibraryImport(library, EntryPoint = "ImGui_ImplWin32_WndProcHandler")]
    private static partial LRESULT ImGuiImplWin32WndProcHandler(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    #endregion

    public IBackend Backend { get; set; } = null!;

    private const string window_class_name_prefix = "astra::window";
    private const byte loop_timer_id = 1;
    private const byte border_width = 8;

    private HWND handle;
    private WNDCLASSEXW wndClass;
    private WindowOptions options;
    private bool running = true;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate managedWndProc = null!;

    public void Setup(in WindowOptions windowOptions)
    {
        options = windowOptions;
        managedWndProc = wndProc;

        switch (options.BackendApi)
        {
            case BackendApi.Auto:
                goto default;
            case BackendApi.Direct3D11:
                Backend = new Direct3D11();
                break;
            default:
                Backend = new Direct3D11();
                break;
        }

        Platform.GetScreenSize(out int monitorViewWidth, out int monitorViewHeight);

        int x = options.StartPosition.X;
        int y = options.StartPosition.Y;

        // Center the window if the position is set to -1
        if (x == -1 && y == -1)
        {
            x = (monitorViewWidth - options.Size.Width) / 2;
            y = (monitorViewHeight - options.Size.Height) / 2;
        }

        // Check if the window is out of bounds and reset to 0 if so
        if (x > monitorViewWidth || x < 0) x = 0;
        if (y > monitorViewHeight || y < 0) y = 0;

        char* pClassName = NativeMemoryUtils.StringToUTF16Ptr($"{window_class_name_prefix}_{Guid.NewGuid()}");
        char* pTitle = NativeMemoryUtils.StringToUTF16Ptr(options.Title);

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

        handle = CreateWindowExW(0, pClassName, pTitle, options.Resizable ? WS.WS_OVERLAPPEDWINDOW : WS.WS_POPUP, x, y, options.Size.Width, options.Size.Height, HWND.NULL, HMENU.NULL, wndClass.hInstance, null);

        NativeMemoryUtils.Free(pClassName);
        NativeMemoryUtils.Free(pTitle);

        Backend.Setup(this);
        Platform.SetWindowStyle(handle, options);

        ShowWindow(handle, SW.SW_SHOW);
        UpdateWindow(handle);
    }


    public void Destroy()
    {
        Backend.Destroy();
        DestroyWindow(handle);
        UnregisterClassW(wndClass.lpszClassName, wndClass.hInstance);
    }


    public void PollEvents()
    {
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
                Backend.Render();
            }
        }
        Destroy();
    }

    public nint GetHandle()
    {
        return handle;
    }

    private LRESULT wndProc(HWND window, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (ImGuiImplWin32WndProcHandler(window, msg, wParam, lParam) > 0) return 1;
        switch (msg)
        {
            case WM.WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                Backend.OnResize(LOWORD(lParam), HIWORD(lParam));
                Backend.Render();
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
                    Backend.Render();

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
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, msg, wParam, lParam);
    }
}