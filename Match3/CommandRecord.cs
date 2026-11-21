using Match3.Canvas;
using Match3.Commands;

namespace Match3;

public sealed record CommandRecord(
    int BrickIndex,
    Brick ActiveBrickBefore,
    Brick ActiveBrickAfter,
    IBrickCommand Command,
    BoardSnapshot BoardBefore,
    BoardSnapshot BoardAfter);
