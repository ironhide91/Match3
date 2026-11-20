using Match3.Canvas;
using System;
using System.Linq;

namespace Match3.ChangeSetGenerator;

public class ClearSelfChangeSetGenerator
{
    public static readonly ClearSelfChangeSetGenerator Instatnce = new();

    private ClearSelfChangeSetGenerator()
    {
        
    }

    public ChangeSet Generate(Brick self)
    {
        // Nothing to erase, because the brick has no previous position on the board.
        var writes = self.Cells
            .Select(c => new CellChange(c.Position.Row, c.Position.Column, '.'))
            .ToList();

        return new ChangeSet(Array.Empty<CellChange>(), writes);
    }
}
