using System.Collections.Generic;

namespace Match3;

public sealed record GameRules(
    int CommandsPerFrame,
    int MaxBrickLength,
    int MaxBricks,
    char EmptyCellSymbol,
    IReadOnlySet<char> AllowedSymbols,
    int MinMatchLength = 3);
