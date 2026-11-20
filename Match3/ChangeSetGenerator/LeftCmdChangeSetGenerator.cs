using Match3.Canvas;
using System.Collections.Generic;
using System.Linq;

namespace Match3.ChangeSetGenerator;

/// <summary>
/// Decides what the board should look like if the active brick moves one column left.
///
/// This class never mutates anything. It only returns a plan (a <see cref="ChangeSet"/>)
/// and <see cref="Board.Apply"/> carries it out. Separating "decide" from "do"
/// keeps the logic easy to unit test.
/// </summary>
public class LeftCmdChangeSetGenerator : ICommandChangeSetGenerator
{
    public ChangeSet Generate(IReadOnlyBoard board, Brick activeBrick)
    {
        // Snapshot of the brick's cells as they are right now.
        var current = activeBrick.Cells.ToList();

        // Fast lookup of the brick's own positions. Used in the obstacle check below so a
        // cell of this brick never counts as an obstacle for another cell of the same brick.
        var occupied = current.Select(c => (c.Position.Row, c.Position.Column)).ToHashSet();

        var writes = new List<CellChange>(current.Count);

        // A brick can only move if ALL of its cells can move, so every cell is checked.
        foreach (var cell in current)
        {
            // Moving left: the row stays the same, the column drops by 1.
            var newColumn = cell.Position   .Column - 1;

            // Wall check: if any cell would leave the board, reject the whole move.
            // Returning Empty means "nothing to do". Being blocked is normal gameplay,
            // so it is not treated as an exception.
            if (newColumn < 0)
                return ChangeSet.Empty;

            // Obstacle check: the destination is blocked only if it holds something
            // AND that something is not part of this same brick.
            // For a brick spanning several columns on one row, a cell's destination is
            // often occupied by a neighbouring cell of the same brick. That neighbour is
            // about to move too, so it must not block the move.
            var blockedByOther =
                board.GetCell(cell.Position.Row, newColumn).Symbol != board.EmptySymbol &&
                !occupied.Contains((cell.Position.Row, newColumn));

            if (blockedByOther)
                return ChangeSet.Empty;

            // This cell can move: record "draw this symbol at the new position".
            //writes.Add(new CellChange(cell.Position.Row + 1, newColumn, cell.Symbol));
            writes.Add(new CellChange(cell.Position.Row, newColumn, cell.Symbol));
        }

        // Record "erase every old position".
        // Board.Apply runs clears BEFORE writes. The order matters because old and new
        // positions can overlap for a multi-column brick. Writing first and clearing
        // second would erase cells that were just drawn.
        var clears = current
            .Select(c => new CellChange(c.Position.Row, c.Position.Column, board.EmptySymbol))
            .ToList();

        return new ChangeSet(clears, writes);

        // NOTE: Generate does not modify the Brick. After board.Apply(changeSet) succeeds
        // (i.e. !changeSet.IsEmpty), the caller must also update the brick's own cell
        // positions (for example with a Brick.MoveBy(0, -1) method), otherwise the next
        // command will be computed from stale positions.
    }
}