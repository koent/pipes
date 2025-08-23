using OpenTK.Mathematics;

namespace Pipes.Structures;

public static class Vector3iExtensions
{
    public static Vector3i Moved(this Vector3i point, Direction direction)
        => new
        (
            point.X + direction switch { Direction.PosX => 1, Direction.NegX => -1, _ => 0 },
            point.Y + direction switch { Direction.PosY => 1, Direction.NegY => -1, _ => 0 },
            point.Z + direction switch { Direction.PosZ => 1, Direction.NegZ => -1, _ => 0 }
        );
}