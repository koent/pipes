using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Pipes.Extensions;
using Pipes.Pipes3D;

namespace Pipes;

public class PipesWindow : GameWindow
{
    public PipesWindow(PipesController controller) : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "Pipes",
        ClientSize = (640, 640),
        NumberOfSamples = 8
    })
    {
        UpdateFrequency = 60;
        _pipesController = controller;
        _shader = new Shader(PipesController.ShaderName);
    }

    private readonly PipesController _pipesController;

    private readonly Shader _shader;

    private float _scale = 1.0f;

    private const float fovDegrees = 45.0f;

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


        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovDegrees), _scale, 0.1f, 100.0f);
        _shader.SetMatrix4("projection", projection);

        var cameraPosition = new Vector3(0.0f, 0.0f, 4.0f);
        var view = Matrix4.CreateTranslation(-cameraPosition);
        _shader.SetMatrix4("view", view);
        _shader.SetVector3("viewPos", cameraPosition);

        _shader.SetVector3("lightDir", new Vector3(3f, 0f, 3f).Normalized());

        ResetPipes();

    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(BeginMode.Triangles, _pipesController.IndexArrayLength, DrawElementsType.UnsignedInt, 0);


        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _pipesController.OnUpdateFrame();

        // Console.Write($"\rFPS: {1/args.Time:F2}, Vertex array length: {_controller.VertexArrayLength:D4}, Index array length: {_controller.IndexArrayLength:D4}, {_controller}");
        // Console.Out.Flush();

        GL.BufferData(BufferTarget.ArrayBuffer, _pipesController.VertexArrayLength * sizeof(float), _pipesController.Vertices, BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _pipesController.IndexArrayLength * sizeof(uint), _pipesController.Indices, BufferUsageHint.StaticDraw);

        if (KeyboardState.IsKeyReleased(Keys.Escape))
        {
            Console.WriteLine();
            Close();
        }
        else if (KeyboardState.IsKeyReleased(Keys.R))
        {
            ResetPipes();
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);
        _scale = (float)args.Width / args.Height;

        ResetPipes();

        GL.Viewport(0, 0, args.Width, args.Height);
    }

    private void ResetPipes()
    {
        _pipesController.Restart(_scale);

        var model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(37))
                  * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(5));
        _shader.SetMatrix4("model", model);

        var normalModel = model.Inverted().Transposed();
        _shader.SetMatrix4("normalModel", normalModel);

        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovDegrees), _scale, 0.1f, 100.0f);
        _shader.SetMatrix4("projection", projection);
    }

    private int VertexBufferObject;

    private int ElementBufferObject;

    private int VertexArrayObject;
}
