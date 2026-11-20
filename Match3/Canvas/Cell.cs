namespace Match3.Canvas;

public sealed record Cell(Position Position, char Symbol)
{
    public Cell Offset(int row, int column)
    {
        return this with
        {
            Position = Position.Offset(row, column)
        };
    }
}