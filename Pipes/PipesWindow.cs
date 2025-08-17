using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Pipes.Pipes2D;

namespace Pipes;

public class PipesWindow : GameWindow
{
    public PipesWindow() : base(GameWindowSettings.Default, new NativeWindowSettings { Title = "Pipes", ClientSize = (640, 640) })
    {
        UpdateFrequency = 60;
    }

    private readonly Controller _controller = new();

    protected override void OnLoad()
    {
        base.OnLoad();

        _controller.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _controller.VertexArrayLength * sizeof(float), _controller.Vertices, BufferUsageHint.StaticDraw);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _controller.IndexArrayLength * sizeof(uint), _controller.Indices, BufferUsageHint.StaticDraw);

        Shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        Shader.Use();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, _controller.IndexArrayLength, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _controller.OnUpdateFrame();

        Console.Write($"\rFPS: {1/args.Time:F2}, Vertex array length: {_controller.VertexArrayLength:D4}, Index array length: {_controller.IndexArrayLength:D4}, {_controller}");
        Console.Out.Flush();

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

        _controller.Restart((float)args.Height / args.Width);

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;

    private Shader Shader;
}