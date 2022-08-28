using System;
using System.Drawing;
using System.Numerics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;
using Pie.Windowing;

namespace PieSamples;

public class Main : SampleApplication
{
    private readonly VertexPositionColor[] _vertices =
    {
        new VertexPositionColor(new Vector3(0.5f, 0.5f, 0), new Vector4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), new Vector4(0, 1, 0, 1)),
        new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), new Vector4(0, 0, 1, 1)),
        new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), new Vector4(0, 0, 0, 1))
    };

    private readonly uint[] _indices =
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

    public override void Initialize()
    {
        _vertexBuffer = Device.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.IndexBuffer, _indices);

        _shader = Device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader));

        _inputLayout = Device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Vec3),
            new InputLayoutDescription("aColor", AttributeType.Vec4));
    }

    public override void Draw()
    {
        Device.SetShader(_shader);
        Device.SetPrimitiveType(PrimitiveType.TriangleList);
        Device.SetVertexBuffer(_vertexBuffer, _inputLayout);
        Device.SetIndexBuffer(_indexBuffer);
        Device.Draw((uint) _indices.Length);
    }

    public override void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        
        base.Dispose();
    }

    public Main(string title) : base(title) { }
}