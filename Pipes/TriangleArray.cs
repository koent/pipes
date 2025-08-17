namespace Pipes;

public class TriangleArray(int maxNofTriangles)
{
    private const int IndicesPerTriangle = 3;

    private readonly uint[] _indices = new uint[maxNofTriangles * IndicesPerTriangle];
    public uint[] Indices => _indices;

    private uint _nofTriangles;
    public int Length => (int)_nofTriangles * IndicesPerTriangle;

    public void Clear()
    {
        _nofTriangles = 0;
    }

    public bool CanAdd(int number = 1) => _nofTriangles + number <= maxNofTriangles;

    public uint Add(uint vertex0, uint vertex1, uint vertex2)
    {
        _indices[IndicesPerTriangle * _nofTriangles + 0] = vertex0;
        _indices[IndicesPerTriangle * _nofTriangles + 1] = vertex1;
        _indices[IndicesPerTriangle * _nofTriangles + 2] = vertex2;

        return _nofTriangles++;
    }
}