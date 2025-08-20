using System;
using System.Reflection.Metadata.Ecma335;
using OpenTK.Mathematics;

namespace Pipes.Structures;

public class VertexArray3D(int maxNofVertices)
{
    private const int FloatsPerVertex = 9;

    private readonly float[] _vertices = new float[maxNofVertices * FloatsPerVertex];
    public float[] Vertices => _vertices;

    private uint _nofVertices;
    public int Length => (int)_nofVertices * FloatsPerVertex;

    public void Clear()
    {
        _nofVertices = 0;
    }

    public bool CanAdd(int number = 1) => _nofVertices + number <= maxNofVertices;

    public uint Add(Vector3 position, Color color)
    {
        _vertices[Length + VertexProperty.X] = position.X;// * _scale;
        _vertices[Length + VertexProperty.Y] = position.Y;
        _vertices[Length + VertexProperty.Z] = position.Z;

        _vertices[Length + VertexProperty.Red] = color.Red;
        _vertices[Length + VertexProperty.Green] = color.Green;
        _vertices[Length + VertexProperty.Blue] = color.Blue;

        _vertices[Length + VertexProperty.DummyX] = 0.2f;
        _vertices[Length + VertexProperty.DummyY] = 0.5f;
        _vertices[Length + VertexProperty.DummyZ] = 0.8f;

        return _nofVertices++;
    }

    public void Move(uint fromBack, Vector3 velocity)
    {
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.X] += velocity.X;// * _scale;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Y] += velocity.Y;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Z] += velocity.Z;
    }
}
