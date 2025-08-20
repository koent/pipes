using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Pipes;

public class PipesWindow : GameWindow
{
    public PipesWindow(IPipesController controller) : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "Pipes",
        ClientSize = (640, 640),
        NumberOfSamples = 8
    })
    {
        UpdateFrequency = 60;
        _controller = controller;
    }

    private readonly IPipesController _controller;

    private float _time = 0.0f;

    protected override void OnLoad()
    {
        base.OnLoad();

        _controller.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
    

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _controller.VertexArrayLength * sizeof(float), _controller.Vertices, BufferUsageHint.StaticDraw);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _controller.IndexArrayLength * sizeof(uint), _controller.Indices, BufferUsageHint.StaticDraw);

        Shader = new Shader
        (
            $"Shaders/{_controller.ShaderName}.vert",
            $"Shaders/{_controller.ShaderName}.frag"
        );

        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 1f, 0.1f, 100.0f);
        Shader.SetMatrix4("projection", projection);



        var view = Matrix4.CreateTranslation(0.0f, 0.0f, -4.0f);
        Shader.SetMatrix4("view", view);

    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Shader.Use();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, _controller.IndexArrayLength, DrawElementsType.UnsignedInt, 0);


        _time += 8.0f * (float)args.Time;
        var model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_time));
        Shader.SetMatrix4("model", model);


        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _controller.OnUpdateFrame();

        // Console.Write($"\rFPS: {1/args.Time:F2}, Vertex array length: {_controller.VertexArrayLength:D4}, Index array length: {_controller.IndexArrayLength:D4}, {_controller}");
        // Console.Out.Flush();

        GL.BufferData(BufferTarget.ArrayBuffer, _controller.VertexArrayLength * sizeof(float), _controller.Vertices, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _controller.IndexArrayLength * sizeof(uint), _controller.Indices, BufferUsageHint.StaticDraw);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Console.WriteLine();
            Close();
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);

        var aspectRatio = (float)args.Width / args.Height;
        _controller.Restart(aspectRatio);

        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), aspectRatio, 0.1f, 100.0f);
        Shader.SetMatrix4("projection", projection);

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;

    private Shader Shader;
}