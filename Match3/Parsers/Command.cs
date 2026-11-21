using Match3.Commands;
using System.Collections.Generic;

namespace Match3.Parsers;

public static partial class InputParser
{
    public static IReadOnlyList<IBrickCommand> ParseFrameCommands(
        string input,
        int maxCommands,
        CommandRegistry registry)
    {
        if (string.IsNullOrEmpty(input))
            return registry.TryGet(' ', out var blank) ? [blank] : [];

        var result = new List<IBrickCommand>(maxCommands);
        foreach (var ch in input)
        {
            if (result.Count == maxCommands)
                break;

            if (registry.TryGet(ch, out var command))
                result.Add(command);
        }

        return result;
    }
}
