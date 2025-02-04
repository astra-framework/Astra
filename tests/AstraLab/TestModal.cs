using System.Drawing;
using System.Numerics;
using Astra.Components;
using Astra.Styles;
using Hexa.NET.ImGui;

namespace AstraLab;

internal static class TestModal
{
    internal const string TITLE = "Test Modal";
    internal static bool Show;

    private static ModalStyle style = new()
    {
        TitlebarFont = Program.font,
        TitlebarBackgroundColor = Color.FromArgb(32, 32, 32),
        TitlebarBorderColor = Color.FromArgb(50, 50, 50),
        BackgroundDimColor = Color.FromArgb(200, 0, 0, 0),
        BackgroundColor = Color.FromArgb(20, 20, 20),
        BorderColor = Color.FromArgb(80, 80, 80),
        Rounding = 3f,
        BorderThickness = 1f,
        TitlebarBorderThickness = 1f,
        TitlebarHeight = 35,
        NoTitlebar = false
    };

    internal static void Render()
    {
        Modal.Normal(TITLE, ref Show, new Vector2(200, 200), style, () =>
        {
            ImGui.Text("AAAAAAAAAAA");
        });
    }
}