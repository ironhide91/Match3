using Match3.Canvas;
using System;
using System.Linq;

namespace Match3;

/// <summary>
/// Moves a freshly parsed brick to its spawn position at the top of the field.
///
/// The spawn rule works out the same way for both orientations: the brick's top edge sits
/// on the top row, and its horizontal extent is centred on the board.
///  - Horizontal bricks occupy the top row only, centred across the board's width.
///  - Vertical bricks occupy the top rows from the top downwards, in the board's middle column.
/// </summary>
public static class SpawnPositioner
{
    public static Brick PlaceAtSpawn(Brick brick, int boardWidth)
    {
        ArgumentNullException.ThrowIfNull(brick);

        if (boardWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(boardWidth));

        var minRow = brick.Cells.Min(c => c.Position.Row);
        var minColumn = brick.Cells.Min(c => c.Position.Column);
        var maxColumn = brick.Cells.Max(c => c.Position.Column);

        // Measured from the cells instead of assuming a length, so the same code
        // centres a horizontal brick, a vertical brick, or any other shape.
        var brickWidth = maxColumn - minColumn + 1;

        // Integer division rounds down. When the leftover space can't be split evenly,
        // the extra empty column ends up on the right.
        var targetLeftColumn = (boardWidth - brickWidth) / 2;

        var rowShift = -minRow;
        var columnShift = targetLeftColumn - minColumn;

        var shifted = brick.Cells
            .Select(c => new Cell(c.Position.Offset(rowShift, columnShift), c.Symbol))
            .ToList();

        // Orientation is carried over unchanged.
        return new Brick(brick.Orientation, shifted);
    }
}