using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Canvas;

public sealed class Brick
{
    private readonly Orientation _orientation;
    private readonly IReadOnlyList<Cell> _cells;

    public Orientation Orientation => _orientation;

    public IReadOnlyList<Cell> Cells => _cells;

    public Brick(Orientation orientation, IReadOnlyList<Cell> cells)
    {
        if (cells == null)
        {
            throw new ArgumentNullException(nameof(cells));
        }

        _cells = [.. cells];

        if (_cells.Count == 0)
        {
            throw new ArgumentException("A brick must contain at least one cell.");
        }

        _orientation = orientation;
    }

    public Brick Offset(int row, int column)
    {
        return new Brick(_orientation, _cells.Select(cell => cell.Offset(row, column)).ToList());
    }
}