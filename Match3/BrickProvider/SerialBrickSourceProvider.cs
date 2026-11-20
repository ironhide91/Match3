using Match3.Canvas;
using System;
using System.Collections.Generic;

namespace Match3.BrickProvider;

public sealed class SerialBrickSourceProvider : IBrickSourceProvider
{
    private IReadOnlyList<Brick> _bricks;
    private int _currentIndex;

    public SerialBrickSourceProvider(IReadOnlyList<Brick> bricks)
    {
        ArgumentNullException.ThrowIfNull(bricks);

        if (bricks.Count == 0)
        {
            throw new ArgumentException("At least one brick is required.");
        }

        _bricks = [.. bricks];
    }

    public bool TryNext(out Brick brick)
    {
        brick = null;

        if (_currentIndex >= _bricks.Count)
        {
            return false;
        }

        brick = _bricks[_currentIndex++];

        return true;
    }
}