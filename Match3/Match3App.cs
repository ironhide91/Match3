using Match3.Canvas;
using Match3.Commands;
using Match3.Parsers;
using System;
using System.IO;
using System.Text;

namespace Match3;

public sealed class Match3App(GameRules rules, CommandRegistry registry, TextReader input, TextWriter output)
{
    public void Run()
    {
        output.WriteLine("Welcome to Match-3 game!");

        while (true)
        {
            var setup = ReadBoardSetup();
            var session = new GameSession(setup, rules);

            PlaySession(session);

            output.WriteLine("Game Over.");
            if (!AskRestart())
            {
                output.WriteLine("Thank you for playing Match-3!");
                break;
            }
        }
    }

    private BoardSetup ReadBoardSetup()
    {
        while (true)
        {
            output.WriteLine($"Please enter field size (width and height) and up to {rules.MaxBricks} bricks set:");
            var line = input.ReadLine();

            if (InputParser.TryParseSetup(line, rules, out var setup))
                return setup;

            output.WriteLine("Invalid input. Example: 5 8 H^^* V*@^");
        }
    }

    private void PlaySession(GameSession session)
    {
        var frame = 0;

        while (session.TrySpawnNextBrick())
        {
            while (session.ActiveBrick is not null)
            {
                RenderFrame(++frame, session.Board);

                output.WriteLine(
                    $"Enter up to {rules.CommandsPerFrame} commands to process before moving to the next frame (valid commands are L,R,D):");

                var rawInput = input.ReadLine();

                // Optional interactive single-command undo ('U') or replay ('P') hooks
                if (string.Equals(rawInput?.Trim(), "U", StringComparison.OrdinalIgnoreCase))
                {
                    if (session.UndoSingleCommand())
                        output.WriteLine("Undid last command.");
                    else
                        output.WriteLine("Nothing to undo.");
                    continue;
                }

                if (string.Equals(rawInput?.Trim(), "P", StringComparison.OrdinalIgnoreCase))
                {
                    output.WriteLine("--- Replaying recorded commands ---");
                    session.Replay((replayFrame, board, rec) =>
                    {
                        if (rec is not null)
                            output.WriteLine($"[Replay Command: {rec.Command.Key}]");
                        RenderFrame(replayFrame, board);
                    });
                    output.WriteLine("--- Replay complete ---");
                    continue;
                }

                var commands = InputParser.ParseFrameCommands(rawInput, rules.CommandsPerFrame, registry);

                foreach (var command in commands)
                {
                    session.TryExecuteSingleCommand(command);
                }

                // Spec: "After commands, the brick always drops by one row automatically.
                // If the brick cannot move further, it becomes stationary."
                session.ApplyGravityAndSettleIfBlocked();
            }
        }

        // Render the final settled / match-cleared board state (e.g., Frame 8 in the spec scenario)
        RenderFrame(++frame, session.Board);
    }

    private void RenderFrame(int frameNumber, Board board)
    {
        output.WriteLine($"Frame {frameNumber}");
        var sb = new StringBuilder(board.Width * 2 + 3);

        for (var r = 0; r < board.Height; r++)
        {
            sb.Clear();
            sb.Append('|');
            for (var c = 0; c < board.Width; c++)
            {
                sb.Append(' ');
                sb.Append(board.GetSymbol(r, c));
            }
            sb.Append(" |");
            output.WriteLine(sb.ToString());
        }
    }

    private bool AskRestart()
    {
        while (true)
        {
            output.WriteLine("Enter S to start over or Q to quit:");
            var answer = input.ReadLine()?.Trim();

            if (string.Equals(answer, "S", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(answer, "Q", StringComparison.OrdinalIgnoreCase) || answer is null)
                return false;
        }
    }
}
