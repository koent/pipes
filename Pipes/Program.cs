namespace Pipes;

class Program
{
    static void Main(string[] args)
    {
        using var pipesWindow = new PipesWindow();
        pipesWindow.Run();
    }
}
