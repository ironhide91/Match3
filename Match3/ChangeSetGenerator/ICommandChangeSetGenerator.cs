using Match3.Canvas;

namespace Match3.ChangeSetGenerator;

public interface ICommandChangeSetGenerator
{
    ChangeSet Generate(IReadOnlyBoard board, Brick activeBrick);
}
