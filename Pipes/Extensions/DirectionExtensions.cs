using System.Collections.Generic;
using OpenTK.Mathematics;
using Pipes.Structures;

namespace Pipes.Extensions;

public static class DirectionExtensions
{
    public static IEnumerable<Direction> Neighbors(this Direction direction)
        => direction switch
        {
            Direction.PosX or Direction.NegX => [Direction.PosY, Direction.NegY, Direction.PosZ, Direction.NegZ],
            Direction.PosY or Direction.NegY => [Direction.PosX, Direction.NegX, Direction.PosZ, Direction.NegZ],
            _ => [Direction.PosY, Direction.NegY, Direction.PosX, Direction.NegX],
        };

    public static Vector3 GetVector(this Direction direction) =>
        direction switch
        {
            Direction.PosX => Vector3.UnitX,
            Direction.NegX => -Vector3.UnitX,
            Direction.PosY => Vector3.UnitY,
            Direction.NegY => -Vector3.UnitY,
            Direction.PosZ => Vector3.UnitZ,
            _ => -Vector3.UnitZ
        };

    public static Vector3 GetPerpendicularVector(this Direction direction)
        => direction is Direction.PosX or Direction.NegX ? Vector3.UnitY : Vector3.UnitX;
}
