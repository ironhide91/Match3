using Match3.Canvas;
using System;
using System.Linq;

namespace Match3.Commands;

public sealed class DropCommand : IBrickCommand
{
    public char Key => 'D';

    public ChangeSet PlanMove(Board board, Brick brick)
    {
        var ownPositions = brick.Cells.Select(c => c.Position).ToHashSet();

        // For each occupied column, scan downward from the brick's lowest cell in that column
        var bottomCells = brick.Cells
            .GroupBy(c => c.Position.Column)
            .Select(g => g.OrderByDescending(c => c.Position.Row).First());

        var dropDistance = int.MaxValue;

        foreach (var cell in bottomCells)
        {
            var free = 0;
            var row = cell.Position.Row + 1;

            while (row < board.Height &&
                   (board.GetSymbol(row, cell.Position.Column) == board.EmptySymbol ||
                    ownPositions.Contains(new Position(row, cell.Position.Column))))
            {
                free++;
                row++;
            }

            dropDistance = Math.Min(dropDistance, free);
            if (dropDistance == 0)
                break;
        }

        return dropDistance == 0
            ? ChangeSet.Empty
            : ChangeSet.FromMove(brick.Cells, rowDelta: dropDistance, colDelta: 0, board.EmptySymbol);
    }
}
