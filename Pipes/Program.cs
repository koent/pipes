using Pipes.Screensaver;

namespace Pipes;

class Program
{
    static void Main(string[] args)
    {
        var options = new ScreensaverOptions(args);

        if (options.Option is ScreensaverOption.Configuration or ScreensaverOption.Preview)
            return;

        using var pipesWindow = new PipesWindow(options);
        pipesWindow.Run();
    }
}
