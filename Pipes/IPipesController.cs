namespace Pipes;

public interface IPipesController
{
    void OnUpdateFrame();

    void Restart(float scale);

    public float[] Vertices { get; }
    public int VertexArrayLength { get; }
    public uint[] Indices { get; }
    public int IndexArrayLength { get; }
    public string ShaderName { get; }
}