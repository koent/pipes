using System;
using OpenTK.Mathematics;
using Pipes.Structures;

namespace Pipes.Extensions;

public static class Vector3Extensions
{
    public static Direction DirectionTowardsOrigin(this Vector3 vector)
    {
        var xAbs = MathF.Abs(vector.X);
        var yAbs = MathF.Abs(vector.Y);
        var zAbs = MathF.Abs(vector.Z);

        var xLargest = yAbs < xAbs && zAbs < xAbs;
        var yLargest = xAbs < yAbs && zAbs < yAbs;

        return xLargest
            ? (vector.X > 0 ? Direction.NegX : Direction.PosX)
            : (yLargest
                ? (vector.Y > 0 ? Direction.NegY : Direction.PosY)
                : (vector.Z > 0 ? Direction.NegZ : Direction.PosZ));
    }
}
