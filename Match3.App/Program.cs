using Match3.Commands;
using System;
using System.Collections.Generic;

namespace Match3.App;

internal class Program
{
    static void Main(string[] args)
    {
        var rules = new GameRules(
            CommandsPerFrame: 2,
            MaxBrickLength: 3,
            MaxBricks: 5,
            EmptyCellSymbol: '.',
            AllowedSymbols: new HashSet<char> { '*', '@', '^' });

        var commands = new CommandRegistry(
        [
            new LeftCommand(),
            new RightCommand(),
            new DropCommand(),
            new BlankCommand()
        ]);

        var game = new Match3App(rules, commands, Console.In, Console.Out);
        game.Run();
    }
}
