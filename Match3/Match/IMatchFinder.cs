using Match3.Canvas;
using System.Collections.Generic;

namespace Match3.Match;

public interface IMatchFinder
{
     bool TryFindMatches(
        int minMatchLength,
        IReadOnlySet<char> allowedSymbols,
        IReadOnlyBoard board,
        out IReadOnlySet<Position> matches);
}
