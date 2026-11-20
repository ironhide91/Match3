using System.Collections.Generic;
using Match3.Canvas;

namespace Match3.Parser;

public static class BrickParser
{
    private const char EmptySymbol = '.';

    /// <summary>
    /// Parses a single brick token. The first character is the orientation ('H' for
    /// horizontal, 'V' for vertical) and every character after it is one cell's symbol,
    /// in order. For example "H^^*" is a horizontal brick whose cells are '^', '^', '*'.
    ///
    /// The brick is built with its first cell at row 0, column 0. Horizontal bricks extend
    /// to the right and vertical bricks extend downwards, with symbols in the order written.
    /// The parser doesn't know the board size or spawn column, so moving the brick to its
    /// real spawn position is left to the spawn logic.
    /// </summary>
    public static bool TryParse(string text, out Brick brick)
    {
        brick = null;

        // Need the orientation plus at least one symbol.
        if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            return false;

        Orientation orientation;
        int rowStep;
        int columnStep;

        switch (char.ToUpperInvariant(text[0]))
        {
            case 'H':
                orientation = Orientation.Horizontal;
                rowStep = 0;
                columnStep = 1;
                break;
            case 'V':
                orientation = Orientation.Vertical;
                rowStep = 1;
                columnStep = 0;
                break;
            default:
                return false;
        }

        var cells = new List<Cell>(text.Length - 1);

        for (var i = 1; i < text.Length; i++)
        {
            var symbol = text[i];

            // Whitespace would break the token format, and '.' is reserved for empty cells,
            // so a brick using it would be indistinguishable from empty space on the board.
            if (char.IsWhiteSpace(symbol) || symbol == EmptySymbol)
                return false;

            var offset = i - 1;
            cells.Add(new Cell(new Position(offset * rowStep, offset * columnStep), symbol));
        }

        brick = new Brick(orientation, cells);
        return true;
    }
}