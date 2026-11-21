using System.Collections.Generic;
using System.Linq;

namespace Match3.Canvas;

public sealed record Brick(Orientation Orientation, IReadOnlyList<Cell> Cells)
{
    public Brick Offset(int rowDelta, int colDelta) =>
        new(Orientation, Cells.Select(c => c.Offset(rowDelta, colDelta)).ToArray());

    public Brick PlaceAtSpawn(int boardWidth)
    {
        var minRow = Cells.Min(c => c.Position.Row);
        var minCol = Cells.Min(c => c.Position.Column);
        var maxCol = Cells.Max(c => c.Position.Column);

        var brickWidth = maxCol - minCol + 1;
        var targetLeftCol = (boardWidth - brickWidth) / 2;

        return Offset(rowDelta: -minRow, colDelta: targetLeftCol - minCol);
    }
}