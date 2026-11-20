using Match3.Canvas;
using System.Collections.Generic;

namespace Match3.Match;

public sealed class MatchFinder : IMatchFinder
{
    public bool TryFindMatches(
        int minMatchLength,
        IReadOnlySet<char> allowedSymbols,
        IReadOnlyBoard board,
        out IReadOnlySet<Position> matches)
    {
        var foundMatches = new HashSet<Position>();

        FindHorizontalMatches(
            board.SettledCells, allowedSymbols, board, foundMatches, minMatchLength);

        FindVerticalMatches(
            board.SettledCells, allowedSymbols, board, foundMatches, minMatchLength);

        matches = foundMatches;

        return foundMatches.Count > 0;
    }

    private static void FindHorizontalMatches(
        IReadOnlySet<Position> settledCells,
        IReadOnlySet<char> allowedSymbols,
        IReadOnlyBoard board,
        HashSet<Position> matches,
        int minMatchLength)
    {
        for (var row = 0; row < board.Height; row++)
        {
            FindMatchesInLine(
                settledCells: settledCells,
                allowedSymbols: allowedSymbols,
                board: board,
                matches: matches,
                startRow: row,
                startColumn: 0,
                rowOffset: 0,
                columnOffset: 1,
                length: board.Width,
                minMatchLength: minMatchLength);
        }
    }

    private static void FindVerticalMatches(
        IReadOnlySet<Position> settledCells,
        IReadOnlySet<char> allowedSymbols,
        IReadOnlyBoard board,
        HashSet<Position> matches,
        int minMatchLength)
    {
        for (var column = 0; column < board.Width; column++)
        {
            FindMatchesInLine(
                settledCells: settledCells,
                allowedSymbols: allowedSymbols,
                board: board,
                matches: matches,
                startRow: 0,
                startColumn: column,
                rowOffset: 1,
                columnOffset: 0,
                length: board.Height,
                minMatchLength: minMatchLength);
        }
    }

    private static void FindMatchesInLine(
        IReadOnlySet<Position> settledCells,
        IReadOnlySet<char> allowedSymbols,
        IReadOnlyBoard board,
        HashSet<Position> matches,
        int startRow,
        int startColumn,
        int rowOffset,
        int columnOffset,
        int length,
        int minMatchLength)
    {
        var line = new List<Position>();
        char? currentSymbol = null;

        for (var i = 0; i < length; i++)
        {
            var row = startRow + i * rowOffset;
            var column = startColumn + i * columnOffset;

            var cell = board.GetCell(row, column);

            // Ignore cells that are not settled or have disallowed symbols.
            if (!settledCells.Contains(cell.Position) ||
                !allowedSymbols.Contains(cell.Symbol))
            {
                AddMatches(line, matches, minMatchLength);
                line.Clear();
                currentSymbol = null;
                continue;
            }

            // Start a new sequence when the symbol changes.
            if (currentSymbol.HasValue && currentSymbol.Value != cell.Symbol)
            {
                AddMatches(line, matches, minMatchLength);
                line.Clear();
            }

            currentSymbol = cell.Symbol;
            line.Add(cell.Position);
        }

        // Process the final sequence.
        AddMatches(line, matches, minMatchLength);
    }

    private static void AddMatches(
        List<Position> line,
        HashSet<Position> matches,
        int minMatchLength)
    {
        if (line.Count >= minMatchLength)
            matches.UnionWith(line);
    }
}