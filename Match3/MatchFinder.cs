using Match3.Canvas;
using System.Collections.Generic;

namespace Match3;

public static class MatchFinder
{
    public static IReadOnlySet<Position> FindMatches(Board board, GameRules rules)
    {
        var matches = new HashSet<Position>();

        // Horizontal lines
        for (var r = 0; r < board.Height; r++)
            ScanLine(board, rules, matches, startRow: r, startCol: 0, rowStep: 0, colStep: 1);

        // Vertical lines
        for (var c = 0; c < board.Width; c++)
            ScanLine(board, rules, matches, startRow: 0, startCol: c, rowStep: 1, colStep: 0);

        // Diagonal lines: top-left to bottom-right (\)
        for (var r = 0; r < board.Height; r++)
            ScanLine(board, rules, matches, startRow: r, startCol: 0, rowStep: 1, colStep: 1);
        for (var c = 1; c < board.Width; c++)
            ScanLine(board, rules, matches, startRow: 0, startCol: c, rowStep: 1, colStep: 1);

        // Diagonal lines: bottom-left to top-right (/)
        for (var r = 0; r < board.Height; r++)
            ScanLine(board, rules, matches, startRow: r, startCol: 0, rowStep: -1, colStep: 1);
        for (var c = 1; c < board.Width; c++)
            ScanLine(board, rules, matches, startRow: board.Height - 1, startCol: c, rowStep: -1, colStep: 1);

        return matches;
    }

    private static void ScanLine(
        Board board,
        GameRules rules,
        HashSet<Position> matches,
        int startRow,
        int startCol,
        int rowStep,
        int colStep)
    {
        var run = new List<Position>();
        char? currentSymbol = null;

        var pos = new Position(startRow, startCol);
        while (board.IsWithinBounds(pos))
        {
            var symbol = board.GetSymbol(pos.Row, pos.Column);
            var isMatchable = board.SettledCells.Contains(pos) && rules.AllowedSymbols.Contains(symbol);

            if (!isMatchable)
            {
                FlushRun(run, matches, rules.MinMatchLength);
                currentSymbol = null;
            }
            else if (currentSymbol == symbol)
            {
                run.Add(pos);
            }
            else
            {
                FlushRun(run, matches, rules.MinMatchLength);
                currentSymbol = symbol;
                run.Add(pos);
            }

            pos = pos.Offset(rowStep, colStep);
        }

        FlushRun(run, matches, rules.MinMatchLength);
    }

    private static void FlushRun(List<Position> run, HashSet<Position> matches, int minLength)
    {
        if (run.Count >= minLength)
            matches.UnionWith(run);
        run.Clear();
    }
}