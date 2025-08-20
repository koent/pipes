namespace Pipes;

class Program
{
    static void Main(string[] args)
    {
        using var pipesWindow = new PipesWindow(new Pipes3D.Controller());
        pipesWindow.Run();
    }
}
