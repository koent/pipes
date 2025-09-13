using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Extensions;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class State
{
    private RasterSet _rasterSet;

    private const int MaxNofVerticesAndTriangles = 64 * 1024;
    private const int SpherePrecision = 20;
    private const int PipePrecision = 20;
    private const float Radius = 0.15f;

    private readonly VertexArray _vertices = new(MaxNofVerticesAndTriangles);
    public float[] Vertices => _vertices.Vertices;
    public int VertexArrayLength => _vertices.Length;

    private readonly TriangleArray _triangles = new(MaxNofVerticesAndTriangles);
    public uint[] Indices => _triangles.Indices;
    public int IndexArrayLength => _triangles.Length;

    private Vector3i _position;
    private Color _color;
    private Direction _direction;

    public bool Started => _vertices.Length > 0;

    public void Clear(int nofPointsXZ, int nofPointsY)
    {
        _vertices.Clear();
        _triangles.Clear();
        _rasterSet = new(nofPointsXZ, nofPointsY, nofPointsXZ);
    }

    public IEnumerable<Direction> NewDirections()
    {
        foreach (var d in _direction.Neighbors())
        {
            var newPosition = _position.Moved(d);
            if (_rasterSet.IsInBounds(newPosition) && !_rasterSet[newPosition])
                yield return d;
        }
    }

    public bool CanStartPipe()
        => _vertices.CanAdd(2 * PipePrecision)
        && _triangles.CanAdd(2 * PipePrecision);
        
    public bool CanStartPipeFrom(Vector3i position, Direction direction)
        => _rasterSet.IsInBounds(position)
        && _rasterSet.IsInBounds(position.Moved(direction))
        && !_rasterSet[position.Moved(direction)];

    public bool CanStartNewPipeFrom(Vector3i position, Direction direction)
        => CanStartPipeFrom(position, direction) && !_rasterSet[position];

    public void StartPipe(Direction direction, Vector3i? position = null, Color? color = null)
    {
        if (!CanStartPipe()) throw new InvalidOperationException("Not enough space to start a new pipe");
        if (!CanStartPipeFrom(position ?? _position, direction)) throw new InvalidOperationException("Cannot start a new pipe");

        _position = position ?? _position;
        _rasterSet[_position] = true;
        _direction = direction;
        _color = color ?? _color;

        AddStartPipes(direction, _position);
    }

    private void AddStartPipes(Direction direction, Vector3i position)
    {
        var firstVertex = _vertices.NofVertices;

        for (int i = 0; i < 2 * PipePrecision; i++)
        {
            var normal = Matrix3.CreateFromAxisAngle(direction.GetVector(), i * MathF.Tau / PipePrecision)
                * direction.GetPerpendicularVector();

            _vertices.Add(position + Radius * normal, _color, normal);
        }

        for (uint i = 0; i < PipePrecision; i++)
        {
            var next = (i + 1) % PipePrecision;
            _triangles.Add(firstVertex + i, firstVertex + next, firstVertex + PipePrecision + i);
            _triangles.Add(firstVertex + next, firstVertex + PipePrecision + i, firstVertex + PipePrecision + next);
        }
    }

    public bool CanTurn() => _vertices.CanAdd((SpherePrecision + 1) * (SpherePrecision / 2 + 1) + 2 * PipePrecision)
        && _triangles.CanAdd(2 * (SpherePrecision - 1) * (SpherePrecision / 2 - 1) + 2 * PipePrecision);

    public void Turn(Direction newDirection, bool bigSphere)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanTurn()) throw new InvalidOperationException("Not enough space to turn");
        if (!NewDirections().Contains(newDirection)) throw new InvalidOperationException("Cannot turn in this direction");
        
        CreateSphere(_position, bigSphere ? 1.5f * Radius : Radius);

        StartPipe(newDirection);
    }

    public bool CanStep()
    {
        var newPosition = _position.Moved(_direction);

        return _rasterSet.IsInBounds(newPosition) && !_rasterSet[newPosition] && Started;
    }

    public void Step(float distance)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanStep()) throw new InvalidOperationException("Cannot step");

        _position = _position.Moved(_direction);
        _rasterSet[_position] = true;

        PerformPartialStep(distance);
    }

    public void PerformPartialStep(float distance)
    {
        for (uint i = 1; i <= PipePrecision; i++)
        {
            _vertices.Move(i, distance * _direction.GetVector());
        }
    }

    private void CreateSphere(Vector3 position, float radius)
    {
        // https://www.songho.ca/opengl/gl_sphere.html#webgl_sphere
        var firstVertex = _vertices.NofVertices;

        const int sectorCount = SpherePrecision;
        const int stackCount = SpherePrecision / 2;

        const float sectorStep = MathF.Tau / sectorCount;
        const float stackStep = MathF.PI / stackCount;

        for (int i = 0; i <= stackCount; i++)
        {
            var xy = MathF.Cos(MathF.PI / 2 - i * stackStep);
            var z = MathF.Sin(MathF.PI / 2 - i * stackStep);

            for (int j = 0; j <= sectorCount; j++)
            {
                var x = xy * MathF.Cos(j * sectorStep);
                var y = xy * MathF.Sin(j * sectorStep);
                var normal = new Vector3(x, y, z);
                _vertices.Add(position + radius * normal, _color, normal);
            }
        }

        for (uint i = 0; i < stackCount; i++)
        {
            uint k1 = i * (sectorCount + 1); // Beginning of current stack
            uint k2 = k1 + sectorCount + 1;  // Beginning of next stack

            for (uint j = 0; j < sectorCount; j++, k1++, k2++)
            {
                if (i != 0)
                    _triangles.Add(firstVertex + k1, firstVertex + k2, firstVertex + k1 + 1);

                if (i != (stackCount - 1))
                    _triangles.Add(firstVertex + k1 + 1, firstVertex + k2, firstVertex + k2 + 1);
            }
        }
    }
}