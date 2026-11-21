using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Canvas;

public sealed class Board
{
    private readonly char[,] _grid;
    private readonly HashSet<Position> _settled = [];

    public int Width { get; }
    public int Height { get; }
    public char EmptySymbol { get; }
    public IReadOnlySet<Position> SettledCells => _settled;

    public Board(int width, int height, char emptySymbol)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Width = width;
        Height = height;
        EmptySymbol = emptySymbol;
        _grid = new char[height, width];

        for (var r = 0; r < height; r++)
            for (var c = 0; c < width; c++)
                _grid[r, c] = emptySymbol;
    }

    public char GetSymbol(int row, int col) => _grid[row, col];

    public bool IsWithinBounds(Position pos) =>
        pos.Row >= 0 && pos.Row < Height && pos.Column >= 0 && pos.Column < Width;

    public bool CanApply(ChangeSet changeSet)
    {
        if (changeSet.IsEmpty)
            return false;

        var clearedPositions = changeSet.Clears.Select(c => c.Position).ToHashSet();

        foreach (var clear in changeSet.Clears)
        {
            if (!IsWithinBounds(clear.Position))
                return false;
        }

        foreach (var write in changeSet.Writes)
        {
            if (!IsWithinBounds(write.Position))
                return false;

            // Destination must be empty unless this changeSet clears it first
            if (_grid[write.Position.Row, write.Position.Column] != EmptySymbol &&
                !clearedPositions.Contains(write.Position))
            {
                return false;
            }
        }

        return true;
    }

    public void Apply(ChangeSet changeSet)
    {
        foreach (var clear in changeSet.Clears)
            _grid[clear.Position.Row, clear.Position.Column] = EmptySymbol;

        foreach (var write in changeSet.Writes)
            _grid[write.Position.Row, write.Position.Column] = write.Symbol;
    }

    public bool CanMoveDown(Brick brick)
    {
        var moveDown = ChangeSet.FromMove(brick.Cells, rowDelta: 1, colDelta: 0, EmptySymbol);
        return CanApply(moveDown);
    }

    public void Settle(Brick brick)
    {
        foreach (var cell in brick.Cells)
            _settled.Add(cell.Position);
    }

    public void ClearPositions(IEnumerable<Position> positions)
    {
        foreach (var pos in positions)
        {
            _grid[pos.Row, pos.Column] = EmptySymbol;
            _settled.Remove(pos);
        }
    }

    public BoardSnapshot CaptureSnapshot()
    {
        var copy = (char[,])_grid.Clone();
        return new BoardSnapshot(copy, [.. _settled]);
    }

    public void RestoreSnapshot(BoardSnapshot snapshot)
    {
        Array.Copy(snapshot.Grid, _grid, _grid.Length);
        _settled.Clear();
        _settled.UnionWith(snapshot.Settled);
    }
}
