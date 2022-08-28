using System;
using System.Diagnostics;
using System.Drawing;
using Pie;
using Pie.Windowing;

namespace PieSamples;

public class SampleApplication : IDisposable
{
    private WindowSettings _settings;

    public Window Window;

    public GraphicsDevice Device;
    
    public SampleApplication(string title)
    {
        _settings = new WindowSettings()
        {
            Title = title,
            Size = new Size(1280, 720),
            Resizable = true
        };
    }

    public virtual void Initialize() { }

    public virtual void Update(float deltaTime) { }

    public virtual void Draw() { }

    public void Run()
    {
        Window = Window.CreateWithGraphicsDevice(_settings, out Device);
        Window.Resize += WindowOnResize;

        Stopwatch dtCounter = Stopwatch.StartNew();
        
        Initialize();

        while (!Window.ShouldClose)
        {
            Window.ProcessEvents();
            
            Update((float) dtCounter.Elapsed.TotalSeconds);
            dtCounter.Restart();
            
            Device.Clear(Color.CornflowerBlue, ClearFlags.Depth | ClearFlags.Stencil);
            Draw();
            
            Device.Present(1);
        }
    }

    private void WindowOnResize(Size size)
    {
        Device.ResizeSwapchain(size);
    }

    public virtual void Dispose()
    {
        Window.Dispose();
        Device.Dispose();
    }
}