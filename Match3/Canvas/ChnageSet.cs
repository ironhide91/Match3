using System.Collections.Generic;
using System.Linq;

namespace Match3.Canvas;

public readonly record struct CellChange(Position Position, char Symbol);

public sealed record ChangeSet(IReadOnlyList<CellChange> Clears, IReadOnlyList<CellChange> Writes)
{
    public static readonly ChangeSet Empty = new([], []);
    public bool IsEmpty => Clears.Count == 0 && Writes.Count == 0;

    public static ChangeSet FromMove(IReadOnlyList<Cell> current, int rowDelta, int colDelta, char emptySymbol)
    {
        var clears = current.Select(c => new CellChange(c.Position, emptySymbol)).ToArray();
        var writes = current.Select(c => new CellChange(c.Position.Offset(rowDelta, colDelta), c.Symbol)).ToArray();
        return new ChangeSet(clears, writes);
    }
}
