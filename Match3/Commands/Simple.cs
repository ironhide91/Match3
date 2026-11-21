using Match3.Canvas;

namespace Match3.Commands;

public sealed class LeftCommand : IBrickCommand
{
    public char Key => 'L';
    public ChangeSet PlanMove(Board board, Brick brick) =>
        ChangeSet.FromMove(brick.Cells, rowDelta: 0, colDelta: -1, board.EmptySymbol);
}

public sealed class RightCommand : IBrickCommand
{
    public char Key => 'R';
    public ChangeSet PlanMove(Board board, Brick brick) =>
        ChangeSet.FromMove(brick.Cells, rowDelta: 0, colDelta: 1, board.EmptySymbol);
}

public sealed class BlankCommand : IBrickCommand
{
    public char Key => ' ';
    public ChangeSet PlanMove(Board board, Brick brick) => ChangeSet.Empty;
}
