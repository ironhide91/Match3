using Match3.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.ChangeSetGenerator;

public class ClearMatchesChangeSetGenerator
{
    public static readonly ClearMatchesChangeSetGenerator Instatnce = new();

    private ClearMatchesChangeSetGenerator()
    {

    }

    public ChangeSet Generate(IReadOnlySet<Position> matches, char emptyCellSymbol)
    {
        var writes = matches
            .Select(c => new CellChange(c.Row, c.Column, emptyCellSymbol))
            .ToList();

        return new ChangeSet(Array.Empty<CellChange>(), writes);
    }
}
