using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Pipes.Pipes3D;

namespace Pipes;

public class PipesWindow : GameWindow
{
    private readonly PipesController _pipesController;
    private readonly ShadingController _shadingController;

    public PipesWindow() : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "Pipes",
        ClientSize = (640, 640),
        NumberOfSamples = 8
    })
    {
        UpdateFrequency = 60;
        _shadingController = new ShadingController();
        _pipesController = new PipesController();
    }

    private float _scale = 1.0f;


    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _pipesController.VertexArrayLength * sizeof(float), _pipesController.Vertices, BufferUsageHint.StaticDraw);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        // Position
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Color
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Normal
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _pipesController.IndexArrayLength * sizeof(uint), _pipesController.Indices, BufferUsageHint.StaticDraw);

        RestartControllers();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // _shadingController.UseShader();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, _pipesController.IndexArrayLength, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var canUpdate = _pipesController.OnUpdateFrame();
        if (!canUpdate)
            RestartControllers();

        // Console.Write($"\rFPS: {1/args.Time:F2}, Vertex array length: {_controller.VertexArrayLength:D4}, Index array length: {_controller.IndexArrayLength:D4}, {_controller}");
        // Console.Out.Flush();

        if (KeyboardState.IsKeyReleased(Keys.Escape))
        {
            Console.WriteLine();
            Close();
        }
        else if (KeyboardState.IsKeyReleased(Keys.R))
        {
            RestartControllers();
        }

        GL.BufferData(BufferTarget.ArrayBuffer, _pipesController.VertexArrayLength * sizeof(float), _pipesController.Vertices, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _pipesController.IndexArrayLength * sizeof(uint), _pipesController.Indices, BufferUsageHint.StaticDraw);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);
        _scale = (float)args.Width / args.Height;

        RestartControllers();

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private void RestartControllers()
    {
        _pipesController.Restart(_scale);
        _shadingController.Restart(_scale);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;
}
