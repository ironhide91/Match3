using System.Collections.Generic;

namespace Match3.Canvas;

public sealed record BoardSetup(
    int Width,
    int Height,
    IReadOnlyList<Brick> Bricks);
