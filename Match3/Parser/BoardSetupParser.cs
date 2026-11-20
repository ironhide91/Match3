using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Match3.Canvas;

namespace Match3.Parser;

public static class BoardSetupParser
{
    /// <summary>
    /// Parses the first input line: board width, board height, then one or more brick tokens,
    /// all separated by whitespace. For example "5 8 H^^* V*@^" is a 5-wide, 8-tall board
    /// with two bricks queued.
    /// Returns false for any malformed line; it never throws on bad input.
    /// </summary>
    public static bool TryParse(string text, out BoardSetup boardSetup)
    {
        boardSetup = null;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        var tokens = text.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);

        // Width, height and at least one brick.
        if (tokens.Length < 3)
            return false;

        if (!TryParsePositiveInt(tokens[0], out var width) ||
            !TryParsePositiveInt(tokens[1], out var height))
            return false;

        var bricks = new List<Brick>(tokens.Length - 2);

        for (var i = 2; i < tokens.Length; i++)
        {
            // One bad brick invalidates the whole line, so the game never starts with a
            // silently shortened brick queue.
            if (!BrickParser.TryParse(tokens[i], out var brick))
                return false;

            bricks.Add(brick);
        }

        boardSetup = new BoardSetup(width, height, bricks);
        return true;
    }

    private static bool TryParsePositiveInt(string token, out int value) =>
        int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out value) && value > 0;
}