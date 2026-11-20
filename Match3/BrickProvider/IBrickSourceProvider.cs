using Match3.Canvas;

namespace Match3.BrickProvider;

public interface IBrickSourceProvider
{
    bool TryNext(out Brick brick);
}
