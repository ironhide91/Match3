using System.Collections.Generic;

namespace Match3.ChangeSetGenerator;

public readonly record struct CellChange(int Row, int Column, char Symbol);

/// <summary>
/// Describes a board mutation. Clears are applied first, then Writes,
/// so a brick moving onto cells it currently occupies works naturally.
/// </summary>
public sealed record ChangeSet(
    IReadOnlyList<CellChange> Clears,
    IReadOnlyList<CellChange> Writes)
{
    public static readonly ChangeSet Empty = new(
        System.Array.Empty<CellChange>(),
        System.Array.Empty<CellChange>());

    public bool IsEmpty => Clears.Count == 0 && Writes.Count == 0;
}
