using System.Collections.Generic;

namespace Match3.Canvas;

public sealed record BoardSetup(int Width, int Height, IReadOnlyList<Brick> Bricks);

public readonly record struct Position(int Row, int Column)
{
    public Position Offset(int rowDelta, int colDelta) => new(Row + rowDelta, Column + colDelta);
}

public readonly record struct Cell(Position Position, char Symbol)
{
    public Cell Offset(int rowDelta, int colDelta) => new(Position.Offset(rowDelta, colDelta), Symbol);
}

public enum Orientation
{
    Horizontal,
    Vertical,
    DiagonalDownRight,
    DiagonalUpRight
}
