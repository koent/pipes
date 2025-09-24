using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Pipes.Structures;

public struct Vertex(Vector3 position, Color color, Vector3 normal, float time)
{
    public readonly IEnumerable<float> GetAttributes()
    {
        yield return position.X;
        yield return position.Y;
        yield return position.Z;
        yield return color.Red;
        yield return color.Green;
        yield return color.Blue;
        yield return normal.X;
        yield return normal.Y;
        yield return normal.Z;
        yield return time;
    }
}
