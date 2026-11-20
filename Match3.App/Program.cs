using Match3.ChangeSetGenerator;
using Match3.Cli;
using Match3.Command;
using Match3.Match;
using System.Linq;

namespace Match3.App;

internal class Program
{
    static void Main(string[] args)
    {
        var rules = new GameRules(
            CommandsPerFrame: 2,
            BrickLength: 3,
            MaxBricks: 5,
            AllowedSymbols: "*@^".ToCharArray().ToHashSet());

        var registry = new CommandRegistry();
        registry.Register(LeftCommand.Instance, new LeftCmdChangeSetGenerator());
        registry.Register(RightCommand.Instance, new RightCmdChangeSetGenerator());
        registry.Register(DropCommand.Instance, new DropCmdChangeSetGenerator());
        registry.Register(BlankCommand.Instance, new BlankCmdChangeSetGenerator());
        
        var config = new GameConfig('.', rules, registry);
        IMatchFinder matchFinder = new MatchFinder();
        IConsole console = new SystemConsole();

        var mainLoop = new MainLoop(config, matchFinder, console);
        mainLoop.Run();
    }
}
