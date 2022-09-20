using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;
using Pie.Windowing;
using Silk.NET.Maths;

namespace PieSamples;

public class Main : SampleApplication
{
    private Vector3[] _cubePos =
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(2.0f, 5.0f, -15.0f),
        new Vector3(-1.5f, -2.2f, -2.5f),
        new Vector3(-3.8f, -2.0f, -12.3f),
        new Vector3(2.4f, -0.4f, -3.5f),
        new Vector3(-1.7f, 3.0f, -7.5f),
        new Vector3(1.3f, -2.0f, -2.5f),
        new Vector3(1.5f, 0.2f, -1.5f),
        new Vector3(-1.3f, 1.0f, -1.5f)
    };

    private const string VertexShader = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjViewTransform
{
    mat4 uProjection;
    mat4 uView;
    mat4 uTransform;
};

void main()
{
    gl_Position = uProjection * uView * uTransform * vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

    private const string FragmentShader = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture1;
layout (binding = 2) uniform sampler2D uTexture2;

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

    private ProjViewTransform _projViewTransform;
    private GraphicsBuffer _transformBuffer;

    private DepthState _depthState;

    private Camera _camera;

    public override void Initialize()
    {
        _vertexBuffer = Device.CreateBuffer(BufferType.VertexBuffer, Cube.Vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.IndexBuffer, Cube.Indices);

        _shader = Device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, VertexShader),
            new ShaderAttachment(ShaderStage.Fragment, FragmentShader));

        _inputLayout = Device.CreateInputLayout(VertexPositionTextureNormal.SizeInBytes,
            new InputLayoutDescription("aPosition", AttributeType.Vec3),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2));

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

        _projViewTransform = new ProjViewTransform();
        _transformBuffer = Device.CreateBuffer(BufferType.UniformBuffer, _projViewTransform, true);

        _depthState = Device.CreateDepthState(DepthStateDescription.LessEqual);

        _camera = new Camera(45, Window.Size.Width / (float) Window.Size.Height);
        _camera.Position = new Vector3(0, 0, -3);

        Window.MouseState = MouseState.Locked;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        const float camSpeed = 20;
        const float mouseSpeed = 0.01f;

        if (IsKeyDown(Keys.W))
            _camera.Position += _camera.Forward * camSpeed * dt;
        if (IsKeyDown(Keys.S))
            _camera.Position -= _camera.Forward * camSpeed * dt;
        if (IsKeyDown(Keys.A))
            _camera.Position -= _camera.Right * camSpeed * dt;
        if (IsKeyDown(Keys.D))
            _camera.Position += _camera.Right * camSpeed * dt;

        _camera.Rotation.X -= DeltaMousePosition.X * mouseSpeed;
        _camera.Rotation.Y -= DeltaMousePosition.Y * mouseSpeed;

        _camera.Rotation.Y = Clamp(_camera.Rotation.Y, -MathF.PI / 2, MathF.PI / 2);

        _projViewTransform.Projection = _camera.ProjectionMatrix;
        _projViewTransform.View = _camera.ViewMatrix;

        if (IsKeyDown(Keys.Escape))
            Window.ShouldClose = true;
    }

    public override void Draw(float dt)
    {
        Device.SetShader(_shader);
        Device.SetUniformBuffer(0, _transformBuffer);
        Device.SetTexture(1, _texture1, _samplerState);
        Device.SetTexture(2, _texture2, _samplerState);
        Device.SetDepthState(_depthState);
        Device.SetPrimitiveType(PrimitiveType.TriangleList);
        Device.SetVertexBuffer(_vertexBuffer, _inputLayout);
        Device.SetIndexBuffer(_indexBuffer);

        for (int i = 0; i < _cubePos.Length; i++)
        {
            Quaternion rotation = Quaternion.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), 20.0f * i);
            _projViewTransform.Transform = Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(rotation)) *
                                           Matrix4x4.CreateTranslation(_cubePos[i]);
            Device.UpdateBuffer(_transformBuffer, 0, _projViewTransform);
            Device.DrawIndexed((uint) Cube.Indices.Length);
        }
    }

    public override void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
        
        base.Dispose();
    }
    
    private struct ProjViewTransform
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
        public Matrix4x4 Transform;

        public ProjViewTransform()
        {
            Projection = Matrix4x4.Identity;
            View = Matrix4x4.Identity;
            Transform = Matrix4x4.Identity;
        }
    }

    public Main(string title) : base(title) { }
}