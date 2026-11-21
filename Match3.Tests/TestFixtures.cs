using Match3.Commands;

namespace Match3.Tests;

internal static class TestFixtures
{
    public static GameRules DefaultRules(
        int commandsPerFrame = 2,
        int brickLength = 3,
        int maxBricks = 5,
        int minMatchLength = 3) =>
        new(
            CommandsPerFrame: commandsPerFrame,
            MaxBrickLength: brickLength,
            MaxBricks: maxBricks,
            EmptyCellSymbol: '.',
            AllowedSymbols: new HashSet<char> { '*', '@', '^' },
            MinMatchLength: minMatchLength);

    public static CommandRegistry DefaultRegistry() =>
        new(
        [
            new LeftCommand(),
            new RightCommand(),
            new DropCommand(),
            new BlankCommand()
        ]);

    public static string RunConsoleSession(GameRules rules, params string[] inputLines)
    {
        var script = string.Join(Environment.NewLine, inputLines) + Environment.NewLine;
        using var reader = new StringReader(script);
        using var writer = new StringWriter();

        var app = new Match3App(rules, DefaultRegistry(), reader, writer);
        app.Run();

        return writer.ToString();
    }
}