using System;
using OpenTK.Mathematics;

namespace Pipes.Structures;

public class RasterSet(int width, int height, int depth)
{
    private bool[] _values = new bool[width * height * depth];

    public bool this[Vector3i point]
    {
        get => this[point.X, point.Y, point.Z];
        set => this[point.X, point.Y, point.Z] = value;
    }

    public bool this[int x, int y, int z]
    {
        get => IsInBounds(x, y, z)
            ? _values[PointToIndex(x, y, z)]
            : throw new IndexOutOfRangeException();
        set
        {
            if (!IsInBounds(x, y, z))
                throw new IndexOutOfRangeException();

            _values[PointToIndex(x, y, z)] = value;
        }
    }

    private int PointToIndex(int x, int y, int z)
        => z * height * width + y * width + x;

    public bool IsInBounds(Vector3i point) => IsInBounds(point.X, point.Y, point.Z);

    public bool IsInBounds(int x, int y, int z)
        => 0 <= x && x < width
        && 0 <= y && y < height
        && 0 <= z && z < depth;

    public void Clear()
    {
        _values = new bool[width * height * depth];
    }
}
