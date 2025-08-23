using OpenTK.Mathematics;

namespace Pipes.Structures;

public class VertexArray(int maxNofVertices)
{
    private const int FloatsPerVertex = 9;

    private readonly float[] _vertices = new float[maxNofVertices * FloatsPerVertex];
    public float[] Vertices => _vertices;

    private uint _nofVertices;
    public uint NofVertices => _nofVertices;
    public int Length => (int)_nofVertices * FloatsPerVertex;

    public void Clear()
    {
        _nofVertices = 0;
    }

    public bool CanAdd(int number = 1) => _nofVertices + number <= maxNofVertices;

    public uint Add(Vector3 position, Color color, Vector3 normal)
    {
        _vertices[Length + VertexProperty.X] = position.X;
        _vertices[Length + VertexProperty.Y] = position.Y;
        _vertices[Length + VertexProperty.Z] = position.Z;

        _vertices[Length + VertexProperty.Red] = color.Red;
        _vertices[Length + VertexProperty.Green] = color.Green;
        _vertices[Length + VertexProperty.Blue] = color.Blue;

        _vertices[Length + VertexProperty.NormalX] = normal.X;
        _vertices[Length + VertexProperty.NormalY] = normal.Y;
        _vertices[Length + VertexProperty.NormalZ] = normal.Z;

        return _nofVertices++;
    }

    public void Move(uint fromBack, Vector3 velocity)
    {
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.X] += velocity.X;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Y] += velocity.Y;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Z] += velocity.Z;
    }
}
