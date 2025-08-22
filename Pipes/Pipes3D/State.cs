using System;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class State(int maxNofVerticesAndTriangles, float pipeRadius, float speed)
{
    private const int SpherePrecision = 24;

    private const int PipePrecision = 12;

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
        var firstVertex = _vertices.NofVertices;

        for (int i = 0; i < 2 * PipePrecision; i++)
        {
            var offset = direction is Direction.North or Direction.South
                ? Matrix3.CreateRotationY(i * MathF.Tau / PipePrecision) * new Vector3(pipeRadius, 0, pipeRadius)
                : Matrix3.CreateRotationX(i * MathF.Tau / PipePrecision) * new Vector3(0, pipeRadius, pipeRadius);

            var normal = offset.Normalized();

            _vertices.Add(position + offset, _color, normal);
        }

        for (uint i = 0; i < PipePrecision; i++)
        {
            var next = (i + 1) % PipePrecision;
            _triangles.Add(firstVertex + i, firstVertex + next, firstVertex + PipePrecision + i);
            _triangles.Add(firstVertex + next, firstVertex + PipePrecision + i, firstVertex + PipePrecision + next);
        }
    }


    public bool CanTurn() => _vertices.CanAdd(SpherePrecision + 1 + 2 * PipePrecision) && _triangles.CanAdd(SpherePrecision + 2 * PipePrecision);

    public void Turn(TurnDirection turnDirection, bool bigSphere)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanTurn()) throw new InvalidOperationException("Not enough space to turn");

        CreateSphere(_position, bigSphere ? 2.0f * pipeRadius : pipeRadius);

        var direction = (Direction)((4 + (int)_direction + (int)turnDirection) % 4);
        StartPipe(direction, _position, _hue, _depth);
    }

    private void CreateSphere(Vector3 position, float radius)
    {
        var fakeNormal = new Vector3(0, 0, 1);
        var center = _vertices.Add(position, _color, fakeNormal);
        var circle = Enumerable.Range(0, SpherePrecision)
            .Select(i => i * 2.0f * MathF.PI / SpherePrecision)
            .Select(angle => new Vector3(radius * MathF.Cos(angle), radius * MathF.Sin(angle), 0))
            .Select(offset => _vertices.Add(position + offset, _color, fakeNormal))
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
    }
}
