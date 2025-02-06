using System.Drawing;
using System.Numerics;
using Astra;
using Astra.Components;
using Astra.Components.Internal;
using Astra.Data;
using Astra.Managers;
using Astra.Platforms.Windows;
using Astra.Styles;
using Astra.Types.Enums;
using Astra.Types.Interfaces;
using Hexa.NET.ImGui;

namespace AstraLab;

internal static class Program
{
    internal static readonly Font font = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inter-Regular.ttf"), 16);
    private static readonly Font font2 = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inter-Regular.ttf"), 18);
    private static readonly Font icon_font = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FontAwesome6.FILE_NAME), 16, [0x0021, 0xF8FF]);

    private static readonly ButtonStyle button_style = new()
    {
        Padding = new Padding(10),
        Font = font,
        TextAlign = TextAlign.Center,
        Display = Display.Flex,

        BorderThickness = 1,
        Radius = 3,
        FadeinSpeed = 400,
        FadeoutSpeed = 400,

        BackgroundColor = Color.FromArgb(62, 62, 62),
        BackgroundHoverColor = Color.FromArgb(82, 82, 82),
        BackgroundActiveColor = Color.FromArgb(22, 22, 22),
        BackgroundDisabledColor = Color.FromArgb(12, 12, 12),

        TextColor = Color.White,
        TextHoverColor = Color.White,
        TextActiveColor = Color.White,
        TextDisabledColor = Color.FromArgb(128, 128, 128),

        BorderColor = Color.FromArgb(50, 50, 50),
        BorderHoverColor = Color.FromArgb(50, 50, 50),
        BorderActiveColor = Color.FromArgb(50, 50, 50),
        BorderDisabledColor = Color.FromArgb(50, 50, 50)
    };

    private static readonly IconButtonStyle icon_style = new()
    {
        Padding = new Padding(10),
        Font = icon_font,
        Display = Display.Block,

        BorderThickness = 1,
        Radius = 3,
        FadeinSpeed = 400,
        FadeoutSpeed = 400,

        BackgroundColor = Color.FromArgb(62, 62, 62),
        BackgroundHoverColor = Color.FromArgb(82, 82, 82),
        BackgroundActiveColor = Color.FromArgb(22, 22, 22),
        BackgroundDisabledColor = Color.FromArgb(12, 12, 12),

        TextColor = Color.White,
        TextHoverColor = Color.White,
        TextActiveColor = Color.White,
        TextDisabledColor = Color.FromArgb(128, 128, 128),

        BorderColor = Color.FromArgb(50, 50, 50),
        BorderHoverColor = Color.FromArgb(50, 50, 50),
        BorderActiveColor = Color.FromArgb(50, 50, 50),
        BorderDisabledColor = Color.FromArgb(50, 50, 50)
    };

    private static readonly TextInputStyle textinput_style = new()
    {
        Padding = new Vector2(8, 4),
        Height = 40,
        Font = font,
        Display = Display.Flex,

        BorderThickness = 1,
        Radius = 3,
        FadeinSpeed = 400,
        FadeoutSpeed = 400,

        BackgroundColor = Color.FromArgb(62, 62, 62),
        BackgroundHoverColor = Color.FromArgb(82, 82, 82),
        BackgroundActiveColor = Color.FromArgb(62, 62, 62),
        BackgroundDisabledColor = Color.FromArgb(12, 12, 12),

        TextColor = Color.White,
        TextHoverColor = Color.White,
        TextActiveColor = Color.White,
        TextDisabledColor = Color.FromArgb(128, 128, 128),

        BorderColor = Color.FromArgb(50, 50, 50),
        BorderHoverColor = Color.FromArgb(50, 50, 50),
        BorderActiveColor = Color.FromArgb(70, 70, 70),
        BorderDisabledColor = Color.FromArgb(50, 50, 50)
    };

    private static readonly PanelStyle main_panel = new()
    {
        BackgroundColor = Color.FromArgb(26, 26, 26),
        BorderThickness = 0,
        Display = Display.Fill,
        Padding = new Vector2(12, 8),
        Radius = 0
    };

    private static readonly TooltipStyle tooltip_style = new()
    {
        Font = font,
        BackgroundColor = Color.FromArgb(32, 32, 32),
        BorderColor = Color.FromArgb(50, 50, 50),
        BorderThickness = 1,
        Padding = new Vector2(4, 4),
        Radius = 3
    };

    private static readonly CheckboxStyle checkboxStyle = new CheckboxStyle
    {
        Font = font,
        Size = 20,
        BorderThickness = 1,
        Radius = 3,
        FadeinSpeed = 400,
        FadeoutSpeed = 400,
        BackgroundColor =  Color.FromArgb(62, 62, 62),
        BackgroundHoverColor = Color.FromArgb(82, 82, 82),
        BackgroundActiveColor = Color.FromArgb(22, 22, 22),
        BackgroundDisabledColor = Color.FromArgb(12, 12, 12),

        CheckColor = Color.White,
        CheckHoverColor = Color.White,
        CheckActiveColor = Color.White,
        CheckDisabledColor = Color.FromArgb(128, 128, 128),

        BorderColor = Color.FromArgb(50, 50, 50),
        BorderHoverColor = Color.FromArgb(50, 50, 50),
        BorderActiveColor = Color.FromArgb(50, 50, 50),
        BorderDisabledColor = Color.FromArgb(50, 50, 50),

        TextColor = Color.White,
        TextHoverColor = Color.White,
        TextActiveColor = Color.White,
        TextDisabledColor = Color.FromArgb(128, 128, 128)
    };

    private static readonly ComboBoxStyle combo_box_style = new()
    {
        Font = font,
        Padding = new Vector2(6, 6),

        TextColor = Color.White,
        BorderColor = Color.FromArgb(80, 80, 80),

        BackgroundColor = Color.FromArgb(32, 32, 32),
        BackgroundHoverColor = Color.FromArgb(42, 42, 42),

        BorderThickness = 1f,
        Radius = 3f,

        DropdownBackgroundColor = Color.FromArgb(42, 42, 42),

        DropdownRadius = 3f,
        DropdownBorderSize = 1f
    };

    private static readonly SelectableComboStyle selectableComboStyle = new()
    {
        Font = font,
        BackgroundHoverColor = Color.FromArgb(100, 100, 100),
        BackgroundActiveColor = Color.FromArgb(60, 60, 60),
    };

    private static void Main()
    {
        FontManager.AddFonts(font, font2, icon_font);
        NotificationManager.SetStyle(new NotificationStyle
        {
            Font = font,
            TextColor = Color.White,
            BackgroundColor = Color.FromArgb(32, 32, 32),
            BorderColor = Color.FromArgb(50, 50, 50),
            Radius = 3f,
            BorderThickness = 1
        });
        IWindow window = Application.CreateWindow(WindowOptions.DEFAULT, new TitlebarStyle
        {
            Height = 40,
            BackgroundColor = Color.FromArgb(32, 32, 32),
            BorderColor = Color.FromArgb(50, 50, 50),
            BorderThickness = 1
        }, onRender, onTitlebarContent);
        window.Poll();
    }


    private static void onTitlebarContent()
    {
        ImGui.SetCursorPos(new Vector2(12, 12));
        Text.Normal("AstraLab", font, Color.White);
    }


    private static string test99 = "333333332";
    private static int test0 = 0;
    private static bool test1 = false;
    private static bool test2 = false;
    private static bool test3 = false;

    private static void onRender()
    {
        Panel.Begin("main_child", main_panel);
        {
            if (Button.Normal("test_button_0", "Hello World", button_style))
            {
                TestModal.Show = true;
                ImGui.OpenPopup(TestModal.TITLE);
            }
            TestModal.Render();
            Tooltip.Normal("test", tooltip_style);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8);
            Text.Normal("Text Input", font2, Color.FromArgb(180, 255, 255, 255));
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
            TextInput.Normal("test01", ref test99, 120, "",textinput_style);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8);
            if (Button.Icon("test_button_1", FontAwesome6.FILE, icon_style))
            {
                test0++;
                NotificationManager.ShowNotification($"Test{test0}", $"Button Pressed! {test0}", 3);
            }
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8);
            if (Checkbox.Normal("##no_label", ref test1, checkboxStyle))
            {
                Console.WriteLine("Checkbox Pressed!");
            }
            ComboBox.Normal("combo_1", "Preview", combo_box_style, () =>
            {
                if (Selectable.Combo("Test0", ref test2, selectableComboStyle))
                {

                }
                if (Selectable.Combo("Test1", ref test3, selectableComboStyle))
                {

                }
            });
        }
        Panel.End();
    }
}