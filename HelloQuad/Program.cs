using System.Drawing;
using HelloQuad;
using Pie.Windowing;

WindowSettings settings = new WindowSettings()
{
    Size = new Size(1280, 720),
    Title = "Hello world!",
    Resizable = true
};

using MainWindow window = new MainWindow(settings);
window.Run();