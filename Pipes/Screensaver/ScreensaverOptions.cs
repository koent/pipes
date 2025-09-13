namespace Pipes.Screensaver;

public class ScreensaverOptions(string[] args)
{
    public ScreensaverOption Option { get; private init; } = args.Length == 0 ? ScreensaverOption.None : args[0][0..2].ToLower() switch
    {
        "/c" => ScreensaverOption.Configuration,
        "/s" => ScreensaverOption.Screensaver,
        "/p" => ScreensaverOption.Preview,
        _ => ScreensaverOption.None
    };
}
