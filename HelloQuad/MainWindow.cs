using System;
using System.Drawing;
using System.Numerics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;
using Pie.Windowing;

namespace HelloQuad;

public class MainWindow : IDisposable
{
    private WindowSettings _settings;

    private Window _window;
    private GraphicsDevice _device;

    private readonly VertexPositionColor[] _vertices = new[]
    {
        new VertexPositionColor(new Vector3(0.5f, 0.5f, 0), new Vector4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), new Vector4(0, 1, 0, 1)),
        new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), new Vector4(0, 0, 1, 1)),
        new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), new Vector4(0, 0, 0, 1))
    };

    private readonly uint[] _indices = new[]
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

layout (location = 0) out vec4 frag_color;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_color = aColor;
}";

    private const string FragmentShader = @"
#version 450

layout (location = 0) in vec4 frag_color;

layout (location = 0) out vec4 out_color;

void main()
{
    out_color = frag_color;
}";

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private Shader _shader;
    private InputLayout _inputLayout;

    public MainWindow(WindowSettings settings)
    {
        _settings = settings;
    }

    public void Initialize()
    {
        _vertexBuffer = _device.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = _device.CreateBuffer(BufferType.IndexBuffer, _indices);

        _shader = _device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader));

        _inputLayout = _device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Vec3),
            new InputLayoutDescription("aColor", AttributeType.Vec4));
    }

    public void Draw()
    {
        _device.Clear(Color.CornflowerBlue);
        
        _device.SetShader(_shader);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetVertexBuffer(_vertexBuffer, _inputLayout);
        _device.SetIndexBuffer(_indexBuffer);
        _device.Draw((uint) _indices.Length);
    }

    private void WindowOnResize(Size size)
    {
        _device.ResizeSwapchain(size);
    }
    
    public void Run()
    {
        _window = Window.CreateWithGraphicsDevice(_settings, out _device);
        _window.Resize += WindowOnResize;
        
        Initialize();

        while (!_window.ShouldClose)
        {
            _window.ProcessEvents();
            Draw();
            _device.Present(1);
        }
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        _device.Dispose();
        _window.Dispose();
    }
}