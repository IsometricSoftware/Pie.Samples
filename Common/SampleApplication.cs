using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using Pie;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace PieSamples;

public class SampleApplication : IDisposable
{
    private string _title;
    private bool _wantsClose;
    private HashSet<Key> _keysDown;

    public Window Window;

    public GraphicsDevice Device;
    
    public SampleApplication(string title)
    {
        _title = title;
        _keysDown = new HashSet<Key>();
    }

    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw(float dt) { }

    public void Run()
    {
        Window = new WindowBuilder()
            .Size(800, 600)
            .Title(_title)
            .Resizable()
            .Build(out Device);

        Stopwatch dtCounter = Stopwatch.StartNew();
        
        Initialize();
        
        while (!_wantsClose)
        {
            DeltaMousePosition = Vector2.Zero;
            
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _wantsClose = true;
                        break;
                    case ResizeEvent resize:
                        WindowOnResize(new Size(resize.Width, resize.Height));
                        break;
                    case KeyEvent key:
                        switch (key.EventType)
                        {
                            case WindowEventType.KeyDown:
                                _keysDown.Add(key.Key);
                                break;
                            case WindowEventType.KeyUp:
                                _keysDown.Remove(key.Key);
                                break;
                        }

                        break;
                    case MouseMoveEvent mouseMove:
                        MousePosition = new Vector2(mouseMove.MouseX, mouseMove.MouseY);
                        DeltaMousePosition += new Vector2(mouseMove.DeltaX, mouseMove.DeltaY);
                        break;
                }
            }

            float dt = (float) dtCounter.Elapsed.TotalSeconds;
            Update(dt);

            Device.ClearColorBuffer(new Vector4(0.2f, 0.3f, 0.3f, 1.0f));
            Device.ClearDepthStencilBuffer(ClearFlags.Depth | ClearFlags.Stencil, 1.0f, 0);
            Draw(dt);
            
            dtCounter.Restart();
            
            Device.Present(1);
        }
    }

    private void WindowOnResize(Size size)
    {
        Device.ResizeSwapchain(size);
        Device.Viewport = new Rectangle(Point.Empty, size);
    }

    public string GetFullPath(string path)
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + path;
    }

    public void Close() => _wantsClose = true;

    public bool IsKeyDown(Key key) => _keysDown.Contains(key);

    public Vector2 MousePosition { get; private set; }
    
    public Vector2 DeltaMousePosition { get; private set; }

    public virtual void Dispose()
    {
        Device.Dispose();
        Window.Dispose();
    }
}