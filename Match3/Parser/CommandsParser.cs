using System.Collections.Generic;
using System.Linq;

namespace Match3.Parser;

public static class CommandsParser
{
    public static bool TryParse(
        string input,
        int maxCommandsPerFrame,
        CommandRegistry registry,
        out IReadOnlyList<CommandHandler> handlers)
    {
        handlers = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            // indiactes a blank command
            input = " ";
        }

        var result = new List<CommandHandler>();

        foreach (var key in input.ToCharArray().Take(maxCommandsPerFrame))
        {
            CommandHandler handler = null;

            if (!registry.TryGet(key, out handler))
            {
                return false;
            }

            result.Add(handler);
        }

        handlers = result;
        return true;
    }
}
