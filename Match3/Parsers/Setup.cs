using Match3.Canvas;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Match3.Parsers;

public static partial class InputParser
{
    public static bool TryParseSetup(string input, GameRules rules, out BoardSetup setup)
    {
        setup = null!;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length < 3 || tokens.Length - 2 > rules.MaxBricks)
            return false;

        if (!int.TryParse(tokens[0], NumberStyles.None, CultureInfo.InvariantCulture, out var width) || width <= 0 ||
            !int.TryParse(tokens[1], NumberStyles.None, CultureInfo.InvariantCulture, out var height) || height <= 0)
            return false;

        var bricks = new List<Brick>(tokens.Length - 2);
        for (var i = 2; i < tokens.Length; i++)
        {
            if (!TryParseBrick(tokens[i], rules, out var brick))
                return false;
            bricks.Add(brick);
        }

        setup = new BoardSetup(width, height, bricks);
        return true;
    }
}
