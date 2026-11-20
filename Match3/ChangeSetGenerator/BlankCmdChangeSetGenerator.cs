using Match3.Canvas;
using System.Collections.Generic;
using System.Linq;

namespace Match3.ChangeSetGenerator;

/// <summary>
/// Decides what the board should look like if the active brick moves one row down.
///
/// This class never mutates anything. It only returns a plan (a ChangeSet),
/// and Board.Apply carries it out.
/// </summary>
public class BlankCmdChangeSetGenerator : ICommandChangeSetGenerator
{
    public ChangeSet Generate(IReadOnlyBoard board, Brick activeBrick)
    {
        var current = activeBrick.Cells;

        var occupied = current
            .Select(c => (c.Position.Row, c.Position.Column))
            .ToHashSet();

        var destinations = new List<CellChange>();

        foreach (var cell in current)
        {
            var row = cell.Position.Row;

            if (row >= board.Height)
                return ChangeSet.Empty;

            var target = board.GetCell(row, cell.Position.Column);

            if (target.Symbol != board.EmptySymbol &&
                !occupied.Contains((row, cell.Position.Column)))
            {
                return ChangeSet.Empty;
            }

            destinations.Add(
                new CellChange(row, cell.Position.Column, cell.Symbol));
        }

        var clears = current
            .Select(c => new CellChange(
                c.Position.Row,
                c.Position.Column,
                board.EmptySymbol))
            .ToList();

        return new ChangeSet(clears, destinations);
    }
}
