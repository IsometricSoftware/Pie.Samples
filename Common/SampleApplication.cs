using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
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
            Size = new Size(800, 600),
            Resizable = true
        };
    }

    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw(float dt) { }

    public void Run()
    {
        Window = Window.CreateWithGraphicsDevice(_settings, out Device);
        Window.Resize += WindowOnResize;

        Stopwatch dtCounter = Stopwatch.StartNew();
        
        Initialize();

        while (!Window.ShouldClose)
        {
            Window.ProcessEvents();

            float dt = (float) dtCounter.Elapsed.TotalSeconds;
            Update(dt);

            Device.Clear(new Vector4(0.2f, 0.3f, 0.3f, 1.0f), ClearFlags.Depth | ClearFlags.Stencil);
            Draw(dt);
            
            dtCounter.Restart();
            
            Device.Present(1);
        }
    }

    private void WindowOnResize(Size size)
    {
        Device.ResizeSwapchain(size);
    }

    public string GetFullPath(string path)
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + path;
    }

    public virtual void Dispose()
    {
        Window.Dispose();
        Device.Dispose();
    }
}