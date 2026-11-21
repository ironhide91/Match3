using System.Collections.Generic;
using System.Linq;

namespace Match3.Commands;

public sealed class CommandRegistry(IEnumerable<IBrickCommand> commands)
{
    private readonly Dictionary<char, IBrickCommand> _map =
        commands.ToDictionary(c => char.ToUpperInvariant(c.Key));

    public bool TryGet(char key, out IBrickCommand command) =>
        _map.TryGetValue(char.ToUpperInvariant(key), out command!);
}
