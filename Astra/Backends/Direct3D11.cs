using System.Drawing;
using System.Numerics;
using Astra.Components.Internal;
using Astra.Managers;
using Astra.Types.Interfaces;
using Astra.Util;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.D3D11;
using Hexa.NET.ImGui.Backends.Win32;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using ID3D11Device = TerraFX.Interop.DirectX.ID3D11Device;
using ID3D11DeviceContext = TerraFX.Interop.DirectX.ID3D11DeviceContext;

namespace Astra.Backends;

public unsafe class Direct3D11 : IBackend
{
    private ID3D11Device* device;
    private ID3D11DeviceContext* context;
    private IDXGISwapChain* swapChain;
    private ID3D11RenderTargetView* renderTargetView;
    private IWindow windowContext = null!;
    private HWND windowHandle;

    private uint backendWidth, backendHeight;
    private bool swapChainOccluded;

    public void Setup(IWindow window)
    {
        windowContext = window;
        windowHandle = (HWND)window.GetHandle();

        DXGI_SWAP_CHAIN_DESC sd = default;
        sd.BufferCount = 2;
        sd.BufferDesc.Width = 0;
        sd.BufferDesc.Height = 0;
        sd.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
        sd.BufferDesc.RefreshRate.Numerator = 60;
        sd.BufferDesc.RefreshRate.Denominator = 1;
        sd.Flags = (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
        sd.BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        sd.OutputWindow = windowHandle;
        sd.SampleDesc.Count = 1;
        sd.SampleDesc.Quality = 0;
        sd.Windowed = true;
        sd.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;

        HRESULT res0 = createDeviceAndSwapChain(sd);
        if (res0 != S.S_OK)
        {
            throw new Exception("Failed to create device and swap chain"); //TODO: Add custom exception
        }

        HRESULT res1 = createRenderTarget();
        if (res1 != S.S_OK)
        {
            throw new Exception("Failed to create render target"); //TODO: Add custom exception
        }

        ImGuiContext* imGuiContext = ImUtils.SetupImGui();
        ImGuiImplWin32.SetCurrentContext(imGuiContext);
        ImGuiImplWin32.Init(windowHandle);
        ImGuiImplD3D11.SetCurrentContext(imGuiContext);
        ImGuiImplD3D11.Init(new ID3D11DevicePtr((Hexa.NET.ImGui.Backends.D3D11.ID3D11Device*)device), new ID3D11DeviceContextPtr((Hexa.NET.ImGui.Backends.D3D11.ID3D11DeviceContext*)context));
    }


    public void Destroy()
    {
        ImGuiImplD3D11.Shutdown();
        ImGuiImplWin32.Shutdown();

        cleanUpRenderTarget();
        if (swapChain != null) swapChain->Release();
        if (context != null) context->Release();
        if (device != null) device->Release();
    }


    public void Poll()
    {
        // Check if the window is occluded and sleep if it is to reduce CPU usage
        if (swapChainOccluded && swapChain->Present(0, DXGI.DXGI_PRESENT_TEST) == DXGI.DXGI_STATUS_OCCLUDED)
        {
            Thread.Sleep(10);
            return;
        }
        swapChainOccluded = false;

        // Resize buffers if it doesn't match
        Size viewPortSize = GetViewportSize();
        if (backendWidth != viewPortSize.Width || backendHeight != viewPortSize.Height)
        {
            cleanUpRenderTarget();
            swapChain->ResizeBuffers(0, backendWidth, backendHeight, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0);
            createRenderTarget();
        }

        ImGuiImplD3D11.NewFrame();
        ImGuiImplWin32.NewFrame();
        ImGui.NewFrame();

        ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new Vector2(backendWidth, backendHeight), ImGuiCond.Always);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, windowContext.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING : Vector2.Zero);
        ImGui.Begin("main", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        {

            ImGui.PopStyleVar();
            Titlebar.WindowsTitlebar(windowContext);
            ImGui.SetCursorPosY(Titlebar.GetHeight(windowContext) + Titlebar.GetBorderThickness());
            ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Transparent.ToVector4());
            // Currently messy workaround for Windows maximized padding, TODO make this cleaner (also in Titlebar.cs and any other places)
            ImGui.SetCursorPosX(OperatingSystem.IsWindows() && windowContext.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING.X : 0);
            ImGui.BeginChild("content_child", new Vector2(backendWidth - (OperatingSystem.IsWindows() && windowContext.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING.X * 2 : 0), backendHeight - Titlebar.GetHeight(windowContext) - (OperatingSystem.IsWindows() && windowContext.IsMaximized() ? Platforms.Windows.Platform.MAXIMIZED_PADDING.Y : 0)), ImGuiChildFlags.AlwaysUseWindowPadding, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            {
                ImGui.PopStyleColor();
                OnRender();
            }
            ImGui.EndChild();
            NotificationManager.RenderNotifications();
        }
        ImGui.End();

        ImGui.Render();
        ID3D11RenderTargetView* lRenderTargetView = renderTargetView;
        context->OMSetRenderTargets(1, &lRenderTargetView, null);
        renderTargetView = lRenderTargetView;
        float[] clearColor = [0.0f, 0.0f, 0.0f, 0.0f];
        fixed (float* pClearColor = clearColor) context->ClearRenderTargetView(renderTargetView, pClearColor);
        ImGuiImplD3D11.RenderDrawData(ImGui.GetDrawData());

        HRESULT hr = swapChain->Present(1, 0);
        swapChainOccluded = hr == DXGI.DXGI_STATUS_OCCLUDED;
    }


    public void OnResize(uint width, uint height)
    {
        backendWidth = width;
        backendHeight = height;
    }


    public Size GetViewportSize()
    {
        DXGI_SWAP_CHAIN_DESC desc;
        swapChain->GetDesc(&desc);
        return new Size((int)desc.BufferDesc.Width, (int)desc.BufferDesc.Height);
    }


    public Action OnRender { get; set; } = null!;


    private HRESULT createDeviceAndSwapChain(DXGI_SWAP_CHAIN_DESC sd)
    {
        HRESULT res;
        const uint create_device_flags = 0;
        D3D_FEATURE_LEVEL[] featureLevelArray = [D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0];
        fixed (D3D_FEATURE_LEVEL* pFeatureLevelArr = featureLevelArray)
        {
            D3D_FEATURE_LEVEL featureLevel;
            ID3D11DeviceContext* lContext;
            ID3D11Device* lDevice;
            IDXGISwapChain* lSwapChain;
            res = DirectX.D3D11CreateDeviceAndSwapChain(null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                HMODULE.NULL,
                create_device_flags,
                pFeatureLevelArr,
                2,
                D3D11.D3D11_SDK_VERSION,
                &sd,
                &lSwapChain,
                &lDevice,
                &featureLevel,
                &lContext);
            // Fallback to WARP if hardware not supported
            if (res == DXGI.DXGI_ERROR_UNSUPPORTED)
            {
                res = DirectX.D3D11CreateDeviceAndSwapChain(null,
                    D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_WARP,
                    HMODULE.NULL,
                    create_device_flags,
                    pFeatureLevelArr,
                    2,
                    D3D11.D3D11_SDK_VERSION,
                    &sd,
                    &lSwapChain,
                    &lDevice,
                    &featureLevel,
                    &lContext);
            }
            if (res != S.S_OK) return res;

            device = lDevice;
            context = lContext;
            swapChain = lSwapChain;
        }

        return res;
    }

    private HRESULT createRenderTarget()
    {
        using ComPtr<ID3D11Resource> backBuffer = null;
        HRESULT gbHr = swapChain->GetBuffer(0, Windows.__uuidof<ID3D11Texture2D>(), (void**)backBuffer.GetAddressOf());
        if (gbHr != S.S_OK) return gbHr;
        ID3D11RenderTargetView* lRenderTargetView;
        HRESULT crtHr = device->CreateRenderTargetView(backBuffer.Get(), null, &lRenderTargetView);
        if (crtHr != S.S_OK) return crtHr;
        renderTargetView = lRenderTargetView;
        return S.S_OK;
    }

    private void cleanUpRenderTarget()
    {
        if (renderTargetView == null) return;
        renderTargetView->Release();
        renderTargetView = null;
    }
}