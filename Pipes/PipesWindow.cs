using System;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Pipes.Pipes3D;
using Pipes.Screensaver;

namespace Pipes;

public class PipesWindow : GameWindow
{
    private readonly ShadingController _shadingController;

    private readonly bool _asScreensaver;
    private int _allowedMouseMoveEvents = 5; // First few mouse move events may have high delta

    public PipesWindow(ScreensaverOptions options) : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "Pipes",
        ClientSize = (640, 640),
        NumberOfSamples = 4,
        WindowState = options.Option == ScreensaverOption.Screensaver ? WindowState.Fullscreen : WindowState.Normal,
    })
    {
        UpdateFrequency = 60;
        _asScreensaver = options.Option == ScreensaverOption.Screensaver;
        _shadingController = new ShadingController(RasterHeight);

        if (_asScreensaver)
        {
            CursorState = CursorState.Hidden;

            KeyUp += _ => Close();
            MouseMove += MouseMoveClose;
        }
    }

    private float _scale = 1.0f;

    private const int RasterHeight = 8;

    private float _duration = 0;
    private const float WaitBeforeResetInSeconds = 5;
    private double _time = 0;

    private void ResetConstruction()
    {
        var pipesConstructor = new PipesConstructor((int)(RasterHeight * _scale), RasterHeight, (int)(RasterHeight * _scale));
        pipesConstructor.Construct();
        var pipePoints = pipesConstructor.Points.ToList();

        var modelConstructor = new ModelConstructor();
        modelConstructor.Construct(pipePoints);

        var vertexAttributes = modelConstructor.GetVertexAttributes().ToArray();
        var indices = modelConstructor.GetIndices().ToArray();

        // Console.WriteLine();
        // Console.WriteLine($"nof points {pipePoints.Count}");
        // Console.WriteLine($"nof vertex attributes {vertexAttributes.Length}");
        // Console.WriteLine($"nof indices {indices.Length}");
        // Console.WriteLine($"nof seconds {modelConstructor.TotalTime}");

        _time = 0;
        _indexArrayLength = indices.Length;
        _duration = modelConstructor.TotalTime;
        _shadingController.SetTime((float)_time);

        GL.BufferData(BufferTarget.ArrayBuffer, vertexAttributes.Length * sizeof(float), vertexAttributes, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
    }

    protected override void OnLoad()
    {
        base.OnLoad();


        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        // Position
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Color
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Normal
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // Time
        GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 10 * sizeof(float), 9 * sizeof(float));
        GL.EnableVertexAttribArray(3);


        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

        RestartControllers();
    }

    private int _indexArrayLength;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, _indexArrayLength, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _shadingController.SetTime((float)_time);
        _time += args.Time;

        // Console.Write($"\rFPS: {1 / args.Time:F2}, {args.Time:F3}");
        // Console.Out.Flush();

        if (_time > _duration + WaitBeforeResetInSeconds)
        {
            RestartControllers();
        }

        if (KeyboardState.IsKeyReleased(Keys.Escape))
        {
            // Console.WriteLine();
            Close();
        }
        else if (KeyboardState.IsKeyReleased(Keys.R))
        {
            RestartControllers();
        }
    }

    private void MouseMoveClose(MouseMoveEventArgs e)
    {
        if (_allowedMouseMoveEvents == 0 && e.Delta.LengthSquared > 10 * 10)
            Close();
     
        if (_allowedMouseMoveEvents > 0)
            _allowedMouseMoveEvents--;
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
        ResetConstruction();
        _shadingController.Restart(_scale);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;
}
