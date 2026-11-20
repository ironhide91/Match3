using Match3.ChangeSetGenerator;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Canvas;

public sealed class Board : IReadOnlyBoard, IDisposable
{
    private readonly Cell[] _cells;
    private readonly int _width;
    private readonly char _emptySymbol;
    private readonly int _height;
    private readonly HashSet<Position> _settled = new();

    public int Width => _width;
    public int Height => _height;
    public char EmptySymbol => _emptySymbol;
    public IReadOnlySet<Cell> Cells => _cells.ToHashSet();
    public IReadOnlySet<Position> SettledCells => _settled;    

    public Board(int height, int width, char emptySymbol)
    {
        _height = height;
        _width = width;
        _emptySymbol = emptySymbol;

        _cells = ArrayPool<Cell>.Shared.Rent(width * height);

        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                _cells[GetIndex(row, column)] = new Cell(new Position(row, column), EmptySymbol);
            }
        }
    }

    public bool Validate(ChangeSet changeSet)
    {
        if (changeSet.IsEmpty)
        {
            return false;
        }

        foreach (var change in changeSet.Clears)
        {
            if (!IsWithinBounds(change.Row, change.Column))
                return false;
        }

        foreach (var change in changeSet.Writes)
        {
            if (!IsWithinBounds(change.Row, change.Column))
                return false;

            // Occupied cells are fine only if this change set clears them first
            // (Apply runs clears before writes).
            if (_cells[GetIndex(change.Row, change.Column)].Symbol != EmptySymbol &&
                !IsCleared(changeSet, change.Row, change.Column))
                return false;
        }

        return true;
    }

    public void Apply(ChangeSet changeSet)
    {
        foreach (var change in changeSet.Clears)
            EnsureWithinBounds(change);

        foreach (var change in changeSet.Writes)
            EnsureWithinBounds(change);

        foreach (var change in changeSet.Clears)
            Set(change.Row, change.Column, EmptySymbol);

        foreach (var change in changeSet.Writes)
            Set(change.Row, change.Column, change.Symbol);
    }

    public bool IsConnected(Brick active)
    {
        bool touches = false;

        foreach (var cell in active.Cells)
        {
            var position = cell.Position;

            // Check if the active brick overlaps an already settled cell.
            // Overlapping is invalid.
            if (_settled.Contains(position))
                return false;

            // Check if the cell touches the bottom boundary of the board.
            // The bottom boundary counts as a connection.
            touches |= position.Row == _height - 1;

            // Check if the cell touches an adjacent settled cell.
            // Only horizontal and vertical adjacency counts.

            // above
            touches |= _settled.Contains(position.Offset(-1, 0));
            // below
            touches |= _settled.Contains(position.Offset(1, 0));
            // left
            touches |= _settled.Contains(position.Offset(0, -1));
            //right
            touches |= _settled.Contains(position.Offset(0, 1));  
        }

        // Return true if at least one cell touches the bottom boundary
        // or an already settled cell.
        return touches;
    }

    public void Settle(Brick brick)
    {
        foreach (var cell in brick.Cells)
            _settled.Add(cell.Position);
    }

    private static bool IsCleared(ChangeSet changeSet, int row, int column)
    {
        foreach (var clear in changeSet.Clears)
        {
            if (clear.Row == row && clear.Column == column)
                return true;
        }

        return false;
    }

    public Cell GetCell(int row, int column) =>
        _cells[GetIndex(row, column)];

    public bool CanMove(Brick brick, int rowOffset, int columnOffset)
    {
        foreach (var cell in brick.Cells)
        {
            var targetRow = cell.Position.Row + rowOffset;
            var targetColumn = cell.Position.Column + columnOffset;

            // Check boundaries
            if (!IsWithinBounds(targetRow, targetColumn))
                return false;

            // Check collision, allowing the brick to overlap its current position
            if (_cells[GetIndex(targetRow, targetColumn)].Symbol != EmptySymbol &&
                !brick.Cells.Any(x =>
                    x.Position.Row == targetRow &&
                    x.Position.Column == targetColumn))
            {
                return false;
            }
        }

        return true;
    }    

    private int GetIndex(int row, int column) =>
        row * _width + column;

    private bool IsWithinBounds(int row, int column) =>
        row >= 0 && row < _height && column >= 0 && column < _width;

    private void Set(int row, int column, char symbol) =>
        _cells[GetIndex(row, column)] = new Cell(new Position(row, column), symbol);

    private void EnsureWithinBounds(CellChange change)
    {
        if (!IsWithinBounds(change.Row, change.Column))
        {
            throw new ArgumentOutOfRangeException(
                nameof(change),
                $"Change at ({change.Row}, {change.Column}) is outside the board.");
        }
    }

    public void Dispose()
    {
        ArrayPool<Cell>.Shared.Return(
            _cells,
            clearArray: true);
    }
}