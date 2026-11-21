using AwesomeAssertions;

namespace Match3.Tests;

internal static class FrameAssertionHelper
{
    /// <summary>
    /// Checks whether any rendered frame in <paramref name="actual"/> matches the
    /// <paramref name="expected"/> board rows (supports either compact row strings like
    /// <c>"....."</c> or rendered frame rows like <c>"| . . . . . |"</c>).
    /// </summary>
    public static bool IsFrameMatch(string? actual, string[] expected)
    {
        if (string.IsNullOrWhiteSpace(actual) || expected is null || expected.Length == 0)
            return false;

        var normalizedExpected = expected.Select(NormalizeRow).ToArray();
        var renderedFrames = ExtractRenderedFrames(actual).Values;

        return renderedFrames.Any(frame =>
            frame.Length == normalizedExpected.Length &&
            frame.SequenceEqual(normalizedExpected, StringComparer.Ordinal));
    }

    /// <summary>
    /// Checks whether a specific <paramref name="frameNumber"/> (e.g. Frame 1, Frame 2)
    /// in <paramref name="actual"/> matches the <paramref name="expected"/> board rows.
    /// </summary>
    public static bool IsFrameMatch(string? actual, int frameNumber, string[] expected)
    {
        if (string.IsNullOrWhiteSpace(actual) || expected is null || expected.Length == 0)
            return false;

        var renderedFrames = ExtractRenderedFrames(actual);
        if (!renderedFrames.TryGetValue(frameNumber, out var frame))
            return false;

        var normalizedExpected = expected.Select(NormalizeRow).ToArray();
        return frame.Length == normalizedExpected.Length &&
               frame.SequenceEqual(normalizedExpected, StringComparer.Ordinal);
    }

    /// <summary>
    /// Extracts all rendered frames keyed by their 1-based frame number.
    /// </summary>
    public static Dictionary<int, string[]> ExtractRenderedFrames(string output)
    {
        var frames = new Dictionary<int, string[]>();
        var currentFrameNumber = 0;
        var currentRows = new List<string>();

        var lines = output.Split(["\r\n", "\n"], StringSplitOptions.None);

        foreach (var rawLine in lines)
        {
            var trimmed = rawLine.Trim();

            if (trimmed.StartsWith("Frame ", StringComparison.Ordinal))
            {
                if (currentFrameNumber > 0 && currentRows.Count > 0)
                {
                    frames[currentFrameNumber] = [.. currentRows];
                    currentRows.Clear();
                }

                if (int.TryParse(trimmed["Frame ".Length..].Trim(), out var parsedFrame))
                    currentFrameNumber = parsedFrame;

                continue;
            }

            if (currentFrameNumber > 0 && trimmed.StartsWith('|') && trimmed.EndsWith('|'))
            {
                currentRows.Add(NormalizeRow(trimmed));
            }
        }

        if (currentFrameNumber > 0 && currentRows.Count > 0)
            frames[currentFrameNumber] = [.. currentRows];

        return frames;
    }

    private static string NormalizeRow(string row)
    {
        var inner = row.Trim();

        if (inner.StartsWith('|'))
            inner = inner[1..];

        if (inner.EndsWith('|'))
            inner = inner[..^1];

        // Collapse spaces so both "| . ^ ^ * . |" and ".^^*." normalize to ".^^*."
        return new string(inner.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
    }
}