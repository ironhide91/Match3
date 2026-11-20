using Match3.Canvas;
using System.Text;

namespace Match3.Cli;

public sealed class FrameRenderer
{
    private const char EmptySymbol = '.';
    private const char Border = '|';

    private readonly IWrite _console;

    internal FrameRenderer(IWrite console)
    {
        _console = console;
    }

    public void Render(int frame, IReadOnlyBoard board)
    {
        _console.WriteLine(string.Empty);
        _console.WriteLine("Frame " + frame);

        // "| " + (2 chars per cell) + "|"
        var line = new StringBuilder(board.Width * 2 + 3);

        for (var row = 0; row < board.Height; row++)
        {
            line.Clear();
            line.Append(Border);

            for (var column = 0; column < board.Width; column++)
            {
                var symbol = board.GetCell(row, column).Symbol;
                line.Append(' ');
                line.Append(symbol == '\0' ? EmptySymbol : symbol);
            }

            line.Append(' ');
            line.Append(Border);

            _console.WriteLine(line.ToString());
        }
    }
}