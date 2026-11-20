using Match3.ChangeSetGenerator;
using Match3.Command;
using System;
using System.Linq;
using System.Collections.Generic;

public sealed record CommandHandler(
    ICommand Command,
    ICommandChangeSetGenerator Generator);

public sealed class CommandRegistry
{
    private readonly Dictionary<char, CommandHandler> _commands = new();

    public void Register(
        ICommand command,
        ICommandChangeSetGenerator generator)
    {
        if (!_commands.TryAdd(command.Key, new(command, generator)))
        {
            throw new InvalidOperationException($"Command '{command.Key}' is already registered.");
        }
    }

    public bool TryGet(char key, out CommandHandler handler)
    {
        return _commands.TryGetValue(key, out handler);
    }

    public IReadOnlyCollection<ICommand> Commands =>
        _commands.Values.Select(x => x.Command).ToArray();
}