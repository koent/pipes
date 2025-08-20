using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class State(int maxNofVerticesAndTriangles, float pipeWidth, float speed)
{
    private const int SpherePrecision = 24;

    private const int PipePrecision = 6;

    private readonly VertexArray3D _vertices = new(maxNofVerticesAndTriangles);
    public float[] Vertices => _vertices.Vertices;
    public int VertexArrayLength => _vertices.Length;

    private readonly TriangleArray _triangles = new(maxNofVerticesAndTriangles);
    public uint[] Indices => _triangles.Indices;
    public int IndexArrayLength => _triangles.Length;

    private float _scale = 1.0f; // In X and Z direction
    private Vector3 _position;
    private Vector3 _velocity;
    private Color _color;
    private float _hue;
    private Direction _direction;
    private float _depth;

    public bool OutOfBounds => MathF.Abs(_position.X) > _scale || MathF.Abs(_position.Y) > 1.0f || MathF.Abs(_position.Y) > _scale;
    private bool Started => _vertices.Length > 0;

    public void Clear(float? newScale = null)
    {
        if (newScale.HasValue)
        {
            _scale = newScale.Value;
        }

        _vertices.Clear();
        _triangles.Clear();
    }

    public bool CanStartPipe() => _vertices.CanAdd(2 * PipePrecision) && _triangles.CanAdd(2 * PipePrecision);

    public void StartPipe(Direction direction, Vector3 position, float hue, float depth)
    {
        if (!CanStartPipe()) throw new InvalidOperationException("Not enough space to start a new pipe");

        _position = position;
        _direction = direction;
        _velocity = direction switch
        {
            Direction.North => (0, speed, 0),
            Direction.East => (speed, 0, 0),
            Direction.South => (0, -speed, 0),
            _ => (-speed, 0, 0),
        };
        _hue = hue;
        _color = Color.FromHue(hue);
        _depth = depth;

        AddStartPipes(direction, position);
    }

    private void AddStartPipes(Direction direction, Vector3 position)
    {
        if (direction is Direction.North or Direction.South)
        {
            List<uint> vertices = [];
            for (int i = 0; i < PipePrecision; i++)
            {
                var v = _vertices.Add(position + (pipeWidth * MathF.Cos(i * MathF.Tau / PipePrecision), 0, pipeWidth * MathF.Sin(i * MathF.Tau / PipePrecision)), _color);
                vertices.Add(v);
            }

            for (int i = 0; i < PipePrecision; i++)
            {
                var v = _vertices.Add(position + (pipeWidth * MathF.Cos(i * MathF.Tau / PipePrecision), 0, pipeWidth * MathF.Sin(i * MathF.Tau / PipePrecision)), _color);
                vertices.Add(v);
            }

            for (int i = 0; i < PipePrecision; i++)
            {
                var next = (i + 1) % PipePrecision;
                _triangles.Add(vertices[i], vertices[next], vertices[PipePrecision + i]);
                _triangles.Add(vertices[next], vertices[PipePrecision + i], vertices[PipePrecision + next]);
            }
        }
        else
        {
            List<uint> vertices = [];
            for (int i = 0; i < PipePrecision; i++)
            {
                var v = _vertices.Add(position + (0, pipeWidth * MathF.Cos(i * MathF.Tau / PipePrecision), pipeWidth * MathF.Sin(i * MathF.Tau / PipePrecision)), _color);
                vertices.Add(v);
            }

            for (int i = 0; i < PipePrecision; i++)
            {
                var v = _vertices.Add(position + (0, pipeWidth * MathF.Cos(i * MathF.Tau / PipePrecision), pipeWidth * MathF.Sin(i * MathF.Tau / PipePrecision)), _color);
                vertices.Add(v);
            }

            for (int i = 0; i < PipePrecision; i++)
            {
                var next = (i + 1) % PipePrecision;
                _triangles.Add(vertices[i], vertices[next], vertices[PipePrecision + i]);
                _triangles.Add(vertices[next], vertices[PipePrecision + i], vertices[PipePrecision + next]);
            }
        }
        return;

        Vector3 pipeOffsetLR = direction is Direction.North or Direction.South ? (pipeWidth, 0, 0) : (0, pipeWidth, 0);
        Vector3 pipeOffsetUD = (0, 0, pipeWidth);

        var begin0 = _vertices.Add(position - pipeOffsetLR + pipeOffsetUD, _color);
        var begin1 = _vertices.Add(position + pipeOffsetLR + pipeOffsetUD, _color);
        var begin2 = _vertices.Add(position + pipeOffsetLR - pipeOffsetUD, _color);
        var begin3 = _vertices.Add(position - pipeOffsetLR - pipeOffsetUD, _color);

        var begin4 = _vertices.Add(position - pipeOffsetLR + pipeOffsetUD, _color);
        var begin5 = _vertices.Add(position + pipeOffsetLR + pipeOffsetUD, _color);
        var begin6 = _vertices.Add(position + pipeOffsetLR - pipeOffsetUD, _color);
        var begin7 = _vertices.Add(position - pipeOffsetLR - pipeOffsetUD, _color);

        _triangles.Add(begin0, begin1, begin4); // 0 1   P+0
        _triangles.Add(begin1, begin4, begin5); // 1 P   P+1

        _triangles.Add(begin1, begin2, begin5); // 1 2   P+1
        _triangles.Add(begin2, begin5, begin6); // 2 P+1 P+2

        _triangles.Add(begin2, begin3, begin6); // 2 3   P+2
        _triangles.Add(begin3, begin6, begin7); // 3 P+2 P+3

        _triangles.Add(begin3, begin0, begin7); // 3 0   P+3
        _triangles.Add(begin0, begin7, begin4); // 0 P+3 P

        // i i+1 P+i
        // i+1 P+i P+i+1
        // alles mod P
    }


    public bool CanTurn() => _vertices.CanAdd(SpherePrecision + 1 + 2 * PipePrecision) && _triangles.CanAdd(SpherePrecision + 2 * PipePrecision);

    public void Turn(TurnDirection turnDirection, bool bigSphere)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanTurn()) throw new InvalidOperationException("Not enough space to turn");

        CreateSphere(_position, bigSphere ? 2.0f * pipeWidth : pipeWidth);

        var direction = (Direction)((4 + (int)_direction + (int)turnDirection) % 4);
        StartPipe(direction, _position, _hue, _depth);
    }

    private void CreateSphere(Vector3 position, float radius)
    {
        var center = _vertices.Add(position, _color);
        var circle = Enumerable.Range(0, SpherePrecision)
            .Select(i => i * 2.0f * MathF.PI / SpherePrecision)
            .Select(angle => new Vector3(radius * MathF.Cos(angle), radius * MathF.Sin(angle), 0))
            .Select(offset => _vertices.Add(position + offset, _color))
            .ToList();

        for (int i = 0; i < SpherePrecision - 1; i++)
        {
            _triangles.Add(center, circle[i], circle[i + 1]);
        }

        _triangles.Add(center, circle[^1], circle[0]);
    }

    public void Step()
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");

        _position += _velocity;


        for (uint i = 1; i <= PipePrecision; i++)
        {
            _vertices.Move(i, _velocity);
        }
        // _vertices.Move(4, _velocity);
        // _vertices.Move(3, _velocity);
        // _vertices.Move(2, _velocity);
        // _vertices.Move(1, _velocity);
    }
}
