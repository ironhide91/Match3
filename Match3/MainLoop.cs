using Match3.BrickProvider;
using Match3.Cli;
using Match3.Match;
using Match3.Parser;
using System;

namespace Match3;

public class MainLoop
{
    private readonly GameConfig _config;
    private readonly IMatchFinder _matchFinder;
    private readonly IConsole _console;    
    private readonly FrameRenderer _frameRenderer;

    public MainLoop(GameConfig config, IMatchFinder matchFinder, IConsole console)
    {
        _config = config;
        _matchFinder = matchFinder;
        _console = console;
        _frameRenderer = new FrameRenderer(console);
    }

    public void Run()
    {
        while (true)
        {
            var setupText = _console.ReadLine();

            if (!BoardSetupParser.TryParse(setupText, out var boardSetup))
            {
                _console.WriteLine("Failed to parse, try again");
                continue;
            }

            var brickProvider = new SerialBrickSourceProvider(boardSetup.Bricks);

            var session = new GameSession(
                boardSetup.Height,
                boardSetup.Width,
                _config.EmptyCellSymbol,
                brickProvider);

            var loop = new GameLoop(
                _config,
                session,
                _matchFinder,
                _console,
                _frameRenderer);

            var result = loop.Run();

            var endOrRestartText = _console.ReadLine();

            if (endOrRestartText.Equals("S", StringComparison.InvariantCultureIgnoreCase))
            {
                _console.WriteLine("Restarting game ...");
                continue;
            }

            if (endOrRestartText.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
            {
                _console.WriteLine("Quiting game ...");
                break;
            }
        }
    }
}
