using System.Drawing;
using Pie;
using Pie.Windowing;

WindowSettings settings = new WindowSettings()
{
    Size = new Size(1280, 720),
    Title = "Learn Pie: Chapter 1 Part 1 - Basic window",
    Border = WindowBorder.Resizable
};
Window window = Window.CreateWithGraphicsDevice(settings, out GraphicsDevice device);
window.Resize += size => device.ResizeSwapchain(size);

while (!window.ShouldClose)
{
    window.ProcessEvents();
    
    device.Clear(Color.CornflowerBlue);
    
    device.Present(1);
}

device.Dispose();
window.Dispose();