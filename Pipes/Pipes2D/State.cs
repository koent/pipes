using System;
using System.Linq;
using Pipes.Structures;

namespace Pipes.Pipes2D;

public class State(int maxNofVerticesAndTriangles, float pipeWidth, float speed)
{
    private const int SpherePrecision = 24;

    private readonly VertexArray _vertices = new(maxNofVerticesAndTriangles);
    public float[] Vertices => _vertices.Vertices;
    public int VertexArrayLength => _vertices.Length;

    private readonly TriangleArray _triangles = new(maxNofVerticesAndTriangles);
    public uint[] Indices => _triangles.Indices;
    public int IndexArrayLength => _triangles.Length;

    private float _scale = 1.0f; // In X direction
    private Vector2 _position;
    private Vector2 _velocity;
    private Color _color;
    private float _hue;
    private Direction _direction;

    public bool OutOfBounds => MathF.Abs(_position.X * _scale) > 1.0f || MathF.Abs(_position.Y) > 1.0f;
    private bool Started => _vertices.Length > 0;

    public void Clear(float? newScale = null)
    {
        if (newScale.HasValue)
        {
            _scale = newScale.Value;
        }

        _vertices.Clear(_scale);
        _triangles.Clear();
    }

    public bool CanStartPipe() => _vertices.CanAdd(4) && _triangles.CanAdd(2);

    public void StartPipe(Direction direction, Vector2 position, float hue)
    {
        if (!CanStartPipe()) throw new InvalidOperationException("Not enough space to start a new pipe");

        _position = position;
        _direction = direction;
        _velocity = direction switch
        {
            Direction.North => (0, speed),
            Direction.East => (speed, 0),
            Direction.South => (0, -speed),
            _ => (-speed, 0),
        };
        _hue = hue;
        _color = Color.FromHue(hue);

        Vector2 pipeOffset = direction is Direction.North or Direction.South ? (pipeWidth, 0) : (0, pipeWidth);

        var begin0 = _vertices.Add(position - pipeOffset, _color);
        var begin1 = _vertices.Add(position + pipeOffset, _color);
        var begin2 = _vertices.Add(position - pipeOffset, _color);
        var begin3 = _vertices.Add(position + pipeOffset, _color);

        _triangles.Add(begin0, begin1, begin2);
        _triangles.Add(begin1, begin2, begin3);
    }

    public bool CanTurn() => _vertices.CanAdd(SpherePrecision + 1 + 4) && _triangles.CanAdd(SpherePrecision + 2);

    public void Turn(TurnDirection turnDirection, bool bigSphere)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanTurn()) throw new InvalidOperationException("Not enough space to turn");

        CreateSphere(_position, bigSphere ? 2.0f * pipeWidth : pipeWidth);

        var direction = (Direction)((4 + (int)_direction + (int)turnDirection) % 4);
        StartPipe(direction, _position, _hue);
    }

    private void CreateSphere(Vector2 position, float radius)
    {
        var center = _vertices.Add(position, _color);
        var circle = Enumerable.Range(0, SpherePrecision)
            .Select(i => i * 2.0f * MathF.PI / SpherePrecision)
            .Select(angle => new Vector2(radius * MathF.Cos(angle), radius * MathF.Sin(angle)))
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

        _vertices.Move(2, _velocity);
        _vertices.Move(1, _velocity);
    }
}
