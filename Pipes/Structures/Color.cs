using System;

namespace Pipes.Structures;

public struct Color(float red, float green, float blue)
{
    public readonly float Red => red;
    public readonly float Green => green;
    public readonly float Blue => blue;

    public static Color FromHue(float hue) => new
    (
        .5f + .5f * MathF.Cos(hue),
        .5f + .5f * MathF.Cos(hue + MathF.PI * 2f / 3f),
        .5f + .5f * MathF.Cos(hue + MathF.PI * 4f / 3f)
    );

    public static Color operator *(float value, Color color)
        => new(value * color.Red, value * color.Green, value * color.Blue);
}
