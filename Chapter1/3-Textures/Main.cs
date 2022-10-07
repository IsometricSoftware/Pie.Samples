using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace PieSamples;

public class Main : SampleApplication
{
    private readonly VertexPositionTexture[] _vertices =
    {
        new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
        new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
        new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
        new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0))
    };

    private readonly uint[] _indices =
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

    private const string FragmentShader = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 0) uniform sampler2D uTexture1;
layout (binding = 1) uniform sampler2D uTexture2;

void main()
{
    vec4 tex1 = texture(uTexture1, frag_texCoords);
    vec4 tex2 = texture(uTexture2, frag_texCoords);
    out_color = mix(tex1, tex2, 0.2);
}";

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private Shader _shader;
    private InputLayout _inputLayout;

    private Texture _texture1;
    private Texture _texture2;
    private SamplerState _samplerState;

    public override void Initialize()
    {
        _vertexBuffer = Device.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.IndexBuffer, _indices);

        _shader = Device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader));

        _inputLayout = Device.CreateInputLayout(VertexPositionTexture.SizeInBytes,
            new InputLayoutDescription("aPosition", AttributeType.Float3),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2));

        TextureDescription textureDesc = new TextureDescription(TextureType.Texture2D, 0, 0, PixelFormat.R8G8B8A8_UNorm,
            true, 1, TextureUsage.ShaderResource);
        
        Bitmap b1 = new Bitmap(GetFullPath("Content/Textures/container.png"));
        textureDesc.Width = b1.Size.Width;
        textureDesc.Height = b1.Size.Height;
        _texture1 = Device.CreateTexture(textureDesc, b1.Data);

        Bitmap b2 = new Bitmap(GetFullPath("Content/Textures/awesomeface.png"));
        textureDesc.Width = b2.Size.Width;
        textureDesc.Height = b2.Size.Height;
        _texture2 = Device.CreateTexture(textureDesc, b2.Data);

        _samplerState = Device.CreateSamplerState(SamplerStateDescription.LinearRepeat);
    }

    public override void Draw(float dt)
    {
        Device.SetShader(_shader);
        Device.SetTexture(0, _texture1, _samplerState);
        Device.SetTexture(1, _texture2, _samplerState);
        Device.SetPrimitiveType(PrimitiveType.TriangleList);
        Device.SetVertexBuffer(_vertexBuffer, _inputLayout);
        Device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        Device.DrawIndexed((uint) _indices.Length);
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