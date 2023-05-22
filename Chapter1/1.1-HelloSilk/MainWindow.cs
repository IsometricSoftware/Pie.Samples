using System;
using System.Drawing;
using Pie;
using Pie.Extensions.SilkWindowing;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace PieSamples;

public class MainWindow : IDisposable
{
    private IWindow _window;
    private GraphicsDevice _device;

    public MainWindow(Size size, string title)
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(size.Width, size.Height);
        options.Title = title;

        // !! IMPORTANT !! Using PieSilk.CreateWindow is not necessary, so make sure you disable options.ShouldSwapAutomatically
        // otherwise Pie might not work properly. It expects you to manually swap buffers yourself using Present().
        // Note: If you use PieSilk.CreateWindow, it will disable this for you.
        
        _window = SilkPie.CreateWindow(ref options);
        _window.Load += Load;
        _window.Render += Render;
        _window.Resize += Resize;
        _window.Closing += Closing;
    }

    private void Load()
    {
        // Create a device on load.
        // You cannot create it before this method is called!
        _device = _window.CreateGraphicsDevice();
    }
    
    private void Render(double obj)
    {
        // Clear the swapchain's color buffer to cornflower blue.
        _device.ClearColorBuffer(Color.CornflowerBlue);
        
        // Present! A swap interval of 1 means that vertical sync is enabled.
        _device.Present(1);
    }
    
    private void Resize(Vector2D<int> obj)
    {
        // Resize the device swapchain on a window resize. If you don't do this, you may get strange results.
        _device.ResizeSwapchain(new Size(obj.X, obj.Y));
    }

    public void Run()
    {
        _window.Run();
    }
    
    private void Closing()
    {
        // Dispose of the device.
        _device.Dispose();
    }

    public void Dispose()
    {
        // And finally dispose of the window.
        // You could also put device disposal here, just make sure to dispose it BEFORE window disposal.
        _window.Dispose();
    }
}