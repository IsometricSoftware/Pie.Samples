using System.Drawing;
using Pie;
using Pie.Windowing;
using Pie.Windowing.Events;

Window window = new WindowBuilder()
    .Size(1280, 720)
    .Title("Learn Pie: Chapter 1 Part 1 - Basic window")
    .Resizable()
    .Build(out GraphicsDevice device);

bool wantsClose = false;
while (!wantsClose)
{
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                wantsClose = true;
                break;
            case ResizeEvent resize:
                device.ResizeSwapchain(new Size(resize.Width, resize.Height));
                break;
        }
    }
    
    device.ClearColorBuffer(Color.CornflowerBlue);
    
    device.Present(1);
}

device.Dispose();
window.Dispose();