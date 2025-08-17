using System;
using System.Linq;

namespace Pipes;

public class State(int maxNofVerticesAndTriangles, float pipeWidth, float speed)
{
    private const int FloatsPerVertex = 6;
    private const int UIntsPerTriangle = 3;
    private const int SpherePrecision = 24;

    public float[] Vertices { get; } = new float[maxNofVerticesAndTriangles * FloatsPerVertex];
    public uint[] Indices { get; } = new uint[maxNofVerticesAndTriangles * UIntsPerTriangle];

    public uint NofVertices;
    public uint NofTriangles;

    public int VertexArrayLength => FloatsPerVertex * (int)NofVertices;
    public int IndexArrayLength => UIntsPerTriangle * (int)NofTriangles;

    public float Scale { get; private set; } = 1.0f; // In X direction

    private bool Started => NofVertices > 0;
    public Vector2 Position { get; set; }
    private Vector2 Velocity { get; set; }
    private Color Color { get; set; }
    private float Hue { get; set; }
    public Direction Direction { get; private set; }

    public bool OutOfBounds => MathF.Abs(Position.X * Scale) > 1.0f || MathF.Abs(Position.Y) > 1.0f;

    public void Clear(float? newScale = null)
    {
        NofVertices = 0;
        NofTriangles = 0;

        if (newScale.HasValue)
        {
            Scale = newScale.Value;
        }
    }

    public bool CanStartPipe()
        => NofVertices + 4 <= maxNofVerticesAndTriangles
        && NofTriangles + 2 <= maxNofVerticesAndTriangles;

    public void StartPipe(Direction direction, Vector2 position, float hue)
    {
        if (!CanStartPipe()) throw new InvalidOperationException("Not enough space to start a new pipe");

        Position = position;
        Direction = direction;
        Velocity = direction switch
        {
            Direction.North => (0, speed),
            Direction.East => (speed, 0),
            Direction.South => (0, -speed),
            _ => (-speed, 0),
        };
        Hue = hue;
        Color = Color.FromHue(hue);

        Vector2 pipeOffset = direction is Direction.North or Direction.South ? (pipeWidth, 0) : (0, pipeWidth);

        var begin0 = AddVertex(position - pipeOffset);
        var begin1 = AddVertex(position + pipeOffset);
        var begin2 = AddVertex(position - pipeOffset);
        var begin3 = AddVertex(position + pipeOffset);

        AddTriangle(begin0, begin1, begin2);
        AddTriangle(begin1, begin2, begin3);
    }

    private uint AddVertex(Vector2 position)
    {
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.X] = position.X * Scale;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.Y] = position.Y;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.Z] = 0.0f;

        Vertices[FloatsPerVertex * NofVertices + VertexProperty.R] = Color.Red;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.G] = Color.Green;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.B] = Color.Blue;

        return NofVertices++;
    }

    private uint AddTriangle(uint vertex0, uint vertex1, uint vertex2)
    {
        Indices[UIntsPerTriangle * NofTriangles + 0] = vertex0;
        Indices[UIntsPerTriangle * NofTriangles + 1] = vertex1;
        Indices[UIntsPerTriangle * NofTriangles + 2] = vertex2;

        return NofTriangles++;
    }

    public bool CanTurn()
        => NofVertices + SpherePrecision + 1 + 4 <= maxNofVerticesAndTriangles
        && NofTriangles + SpherePrecision + 2 <= maxNofVerticesAndTriangles;

    public void Turn(TurnDirection turnDirection, bool sphere)
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");
        if (!CanTurn()) throw new InvalidOperationException("Not enough space to turn");

        CreateSphere(Position, sphere ? 2.0f * pipeWidth : pipeWidth);

        var direction = (Direction)((4 + (int)Direction + (int)turnDirection) % 4);
        StartPipe(direction, Position, Hue);
    }

    private void CreateSphere(Vector2 position, float radius)
    {
        var center = AddVertex(position);
        var circle = Enumerable.Range(0, SpherePrecision)
            .Select(i => i * 2.0f * MathF.PI / SpherePrecision)
            .Select(angle => new Vector2(radius * MathF.Cos(angle), radius * MathF.Sin(angle)))
            .Select(offset => AddVertex(position + offset))
            .ToList();

        for (int i = 0; i < SpherePrecision - 1; i++)
        {
            AddTriangle(center, circle[i], circle[i + 1]);
        }

        AddTriangle(center, circle[^1], circle[0]);
    }

    public void Step()
    {
        if (!Started) throw new InvalidOperationException("Start a pipe first");

        Position += Velocity;

        Vertices[FloatsPerVertex * (NofVertices - 2) + VertexProperty.X] += Velocity.X * Scale;
        Vertices[FloatsPerVertex * (NofVertices - 2) + VertexProperty.Y] += Velocity.Y;

        Vertices[FloatsPerVertex * (NofVertices - 1) + VertexProperty.X] += Velocity.X * Scale;
        Vertices[FloatsPerVertex * (NofVertices - 1) + VertexProperty.Y] += Velocity.Y;
    }
}
