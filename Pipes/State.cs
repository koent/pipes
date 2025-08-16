using System;
using Vec2 = (float X, float Y);
using Color = (float R, float G, float B);
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

    public Vec2 Scale { get; private set; } = (1.0f, 1.0f);

    private bool Started => NofVertices > 0;
    public Vec2 Position { get; set; }
    private Vec2 Velocity { get; set; }
    private Color Color { get; set; }
    private float Hue { get; set; }
    public Direction Direction { get; private set; }

    public bool OutOfBounds => MathF.Abs(Position.X * Scale.X) > 1.0f || MathF.Abs(Position.Y * Scale.Y) > 1.0f;

    public void Clear(float? newScale = null)
    {
        NofVertices = 0;
        NofTriangles = 0;

        if (newScale.HasValue)
        {
            Scale = (newScale.Value, 1.0f);
        }
    }

    public bool CanStartPipe()
        => NofVertices + 4 <= maxNofVerticesAndTriangles
        && NofTriangles + 2 <= maxNofVerticesAndTriangles;

    public void StartPipe(Direction direction, Vec2 position, float hue)
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
        Color =
        (
            .5f + .5f * MathF.Cos(hue),
            .5f + .5f * MathF.Cos(hue + MathF.PI * 2f / 3f),
            .5f + .5f * MathF.Cos(hue + MathF.PI * 4f / 3f)
        );

        uint begin0, begin1, begin2, begin3;

        switch (direction)
        {
            case Direction.North or Direction.South:
                begin0 = AddVertex((position.X - pipeWidth, position.Y));
                begin1 = AddVertex((position.X + pipeWidth, position.Y));
                begin2 = AddVertex((position.X - pipeWidth, position.Y));
                begin3 = AddVertex((position.X + pipeWidth, position.Y));
                break;
            default: // East or West
                begin0 = AddVertex((position.X, position.Y - pipeWidth));
                begin1 = AddVertex((position.X, position.Y + pipeWidth));
                begin2 = AddVertex((position.X, position.Y - pipeWidth));
                begin3 = AddVertex((position.X, position.Y + pipeWidth));
                break;
        }

        AddTriangle(begin0, begin1, begin2);
        AddTriangle(begin1, begin2, begin3);
    }

    private uint AddVertex(Vec2 position)
    {
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.X] = position.X * Scale.X;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.Y] = position.Y * Scale.Y;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.Z] = 0.0f;

        Vertices[FloatsPerVertex * NofVertices + VertexProperty.R] = Color.R;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.G] = Color.G;
        Vertices[FloatsPerVertex * NofVertices + VertexProperty.B] = Color.B;

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

    private void CreateSphere(Vec2 position, float radius)
    {
        var center = AddVertex(position);
        var circle = Enumerable.Range(0, SpherePrecision)
            .Select(i => i * 2.0f * MathF.PI / SpherePrecision)
            .Select(angle => (X: radius * MathF.Cos(angle), Y: radius * MathF.Sin(angle)))
            .Select(offset => AddVertex((position.X + offset.X, position.Y + offset.Y)))
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

        Position = (Position.X + Velocity.X, Position.Y + Velocity.Y);

        Vertices[FloatsPerVertex * (NofVertices - 2) + VertexProperty.X] += Velocity.X * Scale.X;
        Vertices[FloatsPerVertex * (NofVertices - 2) + VertexProperty.Y] += Velocity.Y * Scale.Y;

        Vertices[FloatsPerVertex * (NofVertices - 1) + VertexProperty.X] += Velocity.X * Scale.X;
        Vertices[FloatsPerVertex * (NofVertices - 1) + VertexProperty.Y] += Velocity.Y * Scale.Y;
    }
}
