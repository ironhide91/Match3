using Match3.Canvas;
using System;
using System.Collections.Generic;

namespace Match3.Parsers;

public static partial class InputParser
{
    public static bool TryParseBrick(string token, GameRules rules, out Brick brick)
    {
        brick = null!;
        if (string.IsNullOrWhiteSpace(token) || token.Length != rules.MaxBrickLength + 1)
            return false;

        var (orientation, rowStep, colStep) = char.ToUpperInvariant(token[0]) switch
        {
            'H' => (Orientation.Horizontal, 0, 1),
            'V' => (Orientation.Vertical, 1, 0),
            '\\' => (Orientation.DiagonalDownRight, 1, 1),
            '/' => (Orientation.DiagonalUpRight, -1, 1),
            _ => ((Orientation)(-1), 0, 0)
        };

        if (!Enum.IsDefined(orientation))
            return false;

        var cells = new List<Cell>(rules.MaxBrickLength);
        for (var i = 0; i < rules.MaxBrickLength; i++)
        {
            var symbol = token[i + 1];
            if (!rules.AllowedSymbols.Contains(symbol))
                return false;

            cells.Add(new Cell(new Position(i * rowStep, i * colStep), symbol));
        }

        brick = new Brick(orientation, cells);
        return true;
    }
}
