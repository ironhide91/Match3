using Match3.Canvas;

namespace Match3.Commands;

public interface IBrickCommand
{
    char Key { get; }
    ChangeSet PlanMove(Board board, Brick activeBrick);
}
