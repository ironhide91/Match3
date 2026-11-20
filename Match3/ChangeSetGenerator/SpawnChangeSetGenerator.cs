using Match3.Canvas;
using System;
using System.Linq;

namespace Match3.ChangeSetGenerator;

/// <summary>
/// Builds the change set that draws a newly spawned brick onto the board.
///
/// Unlike the L, R and D generators this is not a command: the brick isn't on the board
/// yet, so there is nothing to clear and no move to validate. It simply writes every cell
/// of the brick at the position it already holds. Placing the brick at its spawn position
/// (for example with Brick.Translate) and checking Board.CanSpawn are the caller's job.
/// </summary>
public static class SpawnChangeSetGenerator
{
    public static ChangeSet Generate(Brick brick)
    {
        // Nothing to erase, because the brick has no previous position on the board.
        var writes = brick.Cells
            .Select(c => new CellChange(c.Position.Row, c.Position.Column, c.Symbol))
            .ToList();

        return new ChangeSet(Array.Empty<CellChange>(), writes);
    }
}
