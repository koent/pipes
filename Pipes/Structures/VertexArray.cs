namespace Pipes.Structures;

public class VertexArray(int maxNofVertices)
{
    private const int FloatsPerVertex = 6;

    private readonly float[] _vertices = new float[maxNofVertices * FloatsPerVertex];
    public float[] Vertices => _vertices;

    private uint _nofVertices;
    public int Length => (int)_nofVertices * FloatsPerVertex;

    private float _scale = 1.0f; // In X direction

    public void Clear(float scale)
    {
        _nofVertices = 0;
        _scale = scale;
    }

    public bool CanAdd(int number = 1) => _nofVertices + number <= maxNofVertices;

    public uint Add(Vector2 position, Color color)
    {
        _vertices[Length + VertexProperty.X] = position.X * _scale;
        _vertices[Length + VertexProperty.Y] = position.Y;
        _vertices[Length + VertexProperty.Z] = 0.0f;

        _vertices[Length + VertexProperty.Red] = color.Red;
        _vertices[Length + VertexProperty.Green] = color.Green;
        _vertices[Length + VertexProperty.Blue] = color.Blue;

        return _nofVertices++;
    }

    public void Move(uint fromBack, Vector2 velocity)
    {
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.X] += velocity.X * _scale;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + VertexProperty.Y] += velocity.Y;
    }
}
