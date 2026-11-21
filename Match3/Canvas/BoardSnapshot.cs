using System.Collections.Generic;

namespace Match3.Canvas;

public sealed record BoardSnapshot(char[,] Grid, HashSet<Position> Settled);
