using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Pipes;

public class PipesWindow : GameWindow
{
    public PipesWindow() : base(GameWindowSettings.Default, new NativeWindowSettings { Title = "Pipes", ClientSize = (640, 640) })
    {
        UpdateFrequency = 60;
    }

    private readonly State State = new(800, 0.02f, 0.05f);

    private readonly Random Random = new();

    private int turnTimer = 0;

    protected override void OnLoad()
    {
        StartRandomPipe();

        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, State.VertexArrayLength * sizeof(float), State.Vertices, BufferUsageHint.StaticDraw);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, State.IndexArrayLength * sizeof(uint), State.Indices, BufferUsageHint.StaticDraw);

        Shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        Shader.Use();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, State.IndexArrayLength, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (turnTimer >= 16 && Random.Next() % 16 == 0)
        {
            if (!State.CanTurn())
            {
                State.Clear();
                StartRandomPipe();
            }

            RandomTurn();
        }

        if (State.OutOfBounds)
        {
            if (!State.CanStartPipe())
                State.Clear();

            StartRandomPipe();
        }

        State.Step();
        turnTimer++;


        Console.Write($"\rTurn timer: {turnTimer:D4}, Direction: {State.Direction.ToString()[0]}, FPS: {1/args.Time:F2}, Vertex array length: {State.VertexArrayLength:D4}, Index array length: {State.IndexArrayLength:D4}");
        Console.Out.Flush();

        GL.BufferData(BufferTarget.ArrayBuffer, State.VertexArrayLength * sizeof(float), State.Vertices, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, State.IndexArrayLength * sizeof(uint), State.Indices, BufferUsageHint.StaticDraw);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Console.WriteLine();
            Close();
        }
    }

    private void StartRandomPipe()
    {
        var x = 2 * Random.NextSingle() - 1;
        var y = 2 * Random.NextSingle() - 1;
        var direction = (Direction)(Random.Next() % 4);
        var hue = 2 * MathF.PI * Random.NextSingle();

        State.StartPipe(direction, (x, y), hue);
    }

    private void RandomTurn()
    {
        var turnDirection = Random.Next() % 2 == 0 ? TurnDirection.Left : TurnDirection.Right;
        var sphere = Random.Next() % 8 == 0;

        State.Turn(turnDirection, sphere);

        turnTimer = 0;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);

        State.Clear((float)args.Height / args.Width);
        StartRandomPipe();

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;

    private Shader Shader;
}