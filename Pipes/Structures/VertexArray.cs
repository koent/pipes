using OpenTK.Mathematics;

namespace Pipes.Structures;

public class VertexArray(int maxNofVertices)
{
    private const int FloatsPerVertex = 10;

    private readonly float[] _vertices = new float[maxNofVertices * FloatsPerVertex];
    public float[] Vertices => _vertices;

    private uint _nofVertices;
    private int _time;
    public uint NofVertices => _nofVertices;
    public int Length => (int)_nofVertices * FloatsPerVertex;

    public void Clear()
    {
        _time = 0;
        _nofVertices = 0;
    }

    public bool CanAdd(int number = 1) => _nofVertices + number <= maxNofVertices;

    public uint Add(Vector3 position, Color color, Vector3 normal)
    {
        _vertices[Length + VertexProperty.Position.X] = position.X;
        _vertices[Length + VertexProperty.Position.Y] = position.Y;
        _vertices[Length + VertexProperty.Position.Z] = position.Z;

        _vertices[Length + VertexProperty.Color.Red] = color.Red;
        _vertices[Length + VertexProperty.Color.Green] = color.Green;
        _vertices[Length + VertexProperty.Color.Blue] = color.Blue;

        _vertices[Length + VertexProperty.Normal.X] = normal.X;
        _vertices[Length + VertexProperty.Normal.Y] = normal.Y;
        _vertices[Length + VertexProperty.Normal.Z] = normal.Z;

        _vertices[Length + VertexProperty.Time] = _time++;

        return _nofVertices++;
    }

    public void Move(uint fromBack, Vector3 velocity)
    {
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Position.X] += velocity.X;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Position.Y] += velocity.Y;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Position.Z] += velocity.Z;
    }
}
