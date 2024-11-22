// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Astra.Data;
using Astra.Platform.Windows;

WindowOptions options = new WindowOptions
{
    Title = "Astra LAB",
    Size = new Size(800, 500)
};
Window window = new Window();
window.Setup(options);
window.PollEvents();