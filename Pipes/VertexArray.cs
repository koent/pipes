namespace Pipes;

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

    public uint Add(Vertex vertex) => Add(vertex.Position, vertex.Color);

    public uint Add(Vector2 position, Color color)
    {
        _vertices[Length + Vertex.Property.X] = position.X * _scale;
        _vertices[Length + Vertex.Property.Y] = position.Y;
        _vertices[Length + Vertex.Property.Z] = 0.0f;

        _vertices[Length + Vertex.Property.Red] = color.Red;
        _vertices[Length + Vertex.Property.Green] = color.Green;
        _vertices[Length + Vertex.Property.Blue] = color.Blue;

        return _nofVertices++;
    }

    public void Move(uint fromBack, Vector2 velocity)
    {
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + Vertex.Property.X] += velocity.X * _scale;
        _vertices[FloatsPerVertex * (_nofVertices - fromBack) + Vertex.Property.Y] += velocity.Y;
    }
}
