using System;
using OpenTK.Mathematics;

namespace Pipes.Pipes3D;

public class ShadingController
{
    private readonly Random _random = new();

    private float _rotationY;
    private float _rotationX;

    public void Restart()
    {
        _rotationY = 90.0f * _random.NextSingle();
        _rotationX = 20.0f * _random.NextSingle();
    }

    public Matrix4 Model => Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY))
                  * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotationX));
}