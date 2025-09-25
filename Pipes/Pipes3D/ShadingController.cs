using System;
using OpenTK.Mathematics;
using Pipes.Extensions;

namespace Pipes.Pipes3D;

public class ShadingController
{
    private const string Name = "pipes3d";
    private const float FovDegrees = 45.0f;

    private readonly Random _random = new();
    private readonly Shader _shader = new(Name);

    private readonly int _rasterHeight;

    public ShadingController(int scaleY)
    {
        _rasterHeight = scaleY;
        var cameraPosition = new Vector3(0.0f, -0.25f, 4.0f);
        var view = Matrix4.CreateTranslation(-cameraPosition);

        _shader.SetMatrix4("view", view);
        _shader.SetVector3("viewPos", cameraPosition);
    }

    public void UseShader() => _shader.Use();

    public void Restart(float scale)
    {
        var rotationY = 90.0f * _random.NextSingle();
        var rotationX = 20.0f * _random.NextSingle();

        var lightX = 2.0f * _random.NextSingle() - 1.0f;
        var lightY = 0.1f + _random.NextSingle();
        var lightZ = _random.NextSingle();


        ResetModel(scale, rotationX, rotationY);
        ResetProjection(scale);
        ResetLight(lightX, lightY, lightZ);
    }

    private void ResetModel(float scale, float rotationX, float rotationY)
    {
        var halfRasterWidth = 0.5f * (int)(_rasterHeight * scale);
        var halfRasterHeight = 0.5f * _rasterHeight;

        var model = Matrix4.CreateTranslation(-halfRasterWidth, -halfRasterHeight, -halfRasterWidth)
                  * Matrix4.CreateScale(1.0f / halfRasterHeight)
                  * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationY))
                  * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotationX));

        var normalModel = model.Inverted().Transposed();
        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("normalModel", normalModel);
    }

    private void ResetLight(float x, float y, float z)
    {
        _shader.SetVector3("lightDir", new Vector3(x, y, z).Normalized());
    }

    private void ResetProjection(float scale)
    {
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FovDegrees), scale, 0.1f, 100.0f);

        _shader.SetMatrix4("projection", projection);
    }

    public void SetTime(float time)
    {
        _shader.SetFloat("time", time);
    }
}
