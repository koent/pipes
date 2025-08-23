using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Pipes.Extensions;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class State(int maxNofVerticesAndTriangles, float pipeRadius, float speed)
{
    private const int SpherePrecision = 20;

    private const int PipePrecision = 20;

    private readonly VertexArray _vertices = new(maxNofVerticesAndTriangles);
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

    public IEnumerable<Direction> TurnDirections => _direction.Neighbors();
    public bool OutOfBounds => MathF.Abs(_position.X) > _scale || MathF.Abs(_position.Y) > 1.0f || MathF.Abs(_position.Y) > _scale;
    public Vector3 Postition => _position;
    private bool Started => _vertices.Length > 0;

    public void Clear(float? newScale = null)
    {
        _scale = newScale ?? _scale;

        _vertices.Clear();
        _triangles.Clear();
    }

    public bool CanStartPipe() => _vertices.CanAdd(2 * PipePrecision) && _triangles.CanAdd(2 * PipePrecision);

    public void StartPipe(Direction direction, Vector3 position, float hue)
    {
        if (!CanStartPipe()) throw new InvalidOperationException("Not enough space to start a new pipe");

        _position = position;
        _direction = direction;
        _velocity = speed * direction.GetVector();
        _hue = hue;
        _color = Color.FromHue(hue);

        AddStartPipes(direction, position);
    }

    private void AddStartPipes(Direction direction, Vector3 position)
    {
        var firstVertex = _vertices.NofVertices;

        for (int i = 0; i < 2 * PipePrecision; i++)
        {
            var normal = Matrix3.CreateFromAxisAngle(direction.GetVector(), i * MathF.Tau / PipePrecision)
                * direction.GetPerpendicularVector();

            _vertices.Add(position + pipeRadius * normal, _color, normal);
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

        CreateSphere(_position, bigSphere ? 1.5f * pipeRadius : pipeRadius);

        StartPipe(newDirection, _position, _hue);
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
