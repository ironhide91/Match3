using System.Collections.Generic;

namespace Match3.Canvas;

public interface IReadOnlyBoard
{
    int Width { get; }
    int Height { get; }
    char EmptySymbol { get; }
    IReadOnlySet<Cell> Cells { get; }
    IReadOnlySet<Position> SettledCells { get; }

    Cell GetCell(int row, int column);
}
