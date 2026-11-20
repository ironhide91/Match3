using System.Collections.Generic;

namespace Match3;

public sealed record GameRules(
    int CommandsPerFrame,
    int BrickLength,
    int MaxBricks,
    IReadOnlySet<char> AllowedSymbols);
