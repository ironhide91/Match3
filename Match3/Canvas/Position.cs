namespace Match3.Canvas;

public readonly record struct Position(int Row, int Column)
{
    public Position Offset(int row, int column)
    {
        return this with
        {
            Row = Row + row,
            Column = Column + column
        };
    }
}
