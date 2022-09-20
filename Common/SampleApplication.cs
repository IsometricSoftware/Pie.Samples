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
    private InputState _state;
    private Vector2 _prevPos;

    public Window Window;

    public GraphicsDevice Device;
    
    public SampleApplication(string title)
    {
        _settings = new WindowSettings()
        {
            Title = title,
            Size = new Size(800, 600),
            Border = WindowBorder.Resizable
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

        _state = Window.ProcessEvents();

        while (!Window.ShouldClose)
        {
            _prevPos = _state.MousePosition;
            _state = Window.ProcessEvents();
            DeltaMousePosition = _state.MousePosition - _prevPos;

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

    public bool IsKeyDown(Keys key) => _state.IsKeyDown(key);

    public Vector2 MousePosition => _state.MousePosition;
    
    public Vector2 DeltaMousePosition { get; private set; }

    public float Clamp(float value, float min, float max) => value <= min ? min : value >= max ? max : value;

    public virtual void Dispose()
    {
        Device.Dispose();
        Window.Dispose();
    }
}