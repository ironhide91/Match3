using Match3.Canvas;
using System;
using System.Linq;

namespace Match3.ChangeSetGenerator;

/// <summary>
/// Decides what the board should look like if the active brick is dropped as far down
/// as possible, stopping at the bottom of the board or directly above any existing brick.
///
/// Orientation matters for this command. Dropping only collides with whatever is beneath
/// the brick, so for each column the brick occupies, only its lowest cell is scanned:
///  - Vertical brick: one column, so only the bottom cell is scanned.
///  - Horizontal brick: several columns, each with a single cell, so every cell is scanned
///    and the column with the least free space decides how far the whole brick can fall.
///  - Any other shape: handled the same way, one scan per occupied column.
///
/// This class never mutates anything. It only returns a plan (a <see cref="ChangeSet"/>)
/// and <see cref="Board.Apply"/> carries it out.
/// </summary>
public class DropCmdChangeSetGenerator : ICommandChangeSetGenerator
{
    public ChangeSet Generate(IReadOnlyBoard board, Brick activeBrick)
    {
        // Snapshot of the brick's cells as they are right now.
        var current = activeBrick.Cells.ToList();

        // Lowest cell of each occupied column. Row numbers grow downwards, so the lowest
        // cell is the one with the highest row. Everything beneath it in that column is
        // outside the brick, so no "own cells" exception is needed when scanning.
        var bottomCells = current
            .GroupBy(c => c.Position.Column)
            .Select(g => g.OrderByDescending(c => c.Position.Row).First());

        // How many rows the whole brick can fall. The brick moves as one rigid piece, so the
        // column with the least free space below it limits the drop for every cell.
        var dropDistance = int.MaxValue;

        foreach (var cell in bottomCells)
        {
            // Count the empty cells directly beneath this column's lowest cell, stopping
            // at the first occupied cell or at the floor.
            var free = 0;
            var row = cell.Position.Row + 1;

            while (row < board.Height && board.GetCell(row, cell.Position.Column).Symbol == board.EmptySymbol)
            {
                free++;
                row++;
            }

            dropDistance = Math.Min(dropDistance, free);

            // Early exit: a distance of zero cannot get any smaller.
            if (dropDistance == 0)
                break;
        }

        // The brick is already resting on the floor or on another brick, so nothing moves.
        // Returning Empty means "nothing to do". Landing is normal gameplay, so it is not
        // treated as an exception.
        if (dropDistance == 0)
            return ChangeSet.Empty;

        // Every cell of the brick moves down by the same distance, keeping its column and symbol.
        var writes = current
            .Select(c => new CellChange(c.Position.Row + dropDistance, c.Position.Column, c.Symbol))
            .ToList();

        // Erase every old position.
        // Board.Apply runs clears BEFORE writes. The order matters because for a tall brick
        // or a short drop the new positions can overlap the old ones. Writing first and
        // clearing second would erase cells that were just drawn.
        var clears = current
            .Select(c => new CellChange(c.Position.Row, c.Position.Column, board.EmptySymbol))
            .ToList();

        return new ChangeSet(clears, writes);

        // NOTE: Generate does not modify the Brick. After board.Apply(changeSet) succeeds
        // (i.e. !changeSet.IsEmpty), the caller must also update the brick's own cell
        // positions by the same distance, otherwise the next command will be computed from
        // stale positions. The distance can be recovered from any write, for example
        // writes[0].Row - current[0].Row. A drop also leaves the brick resting on something,
        // so this is the point where the game should lock it in place and spawn the next one.
    }
}