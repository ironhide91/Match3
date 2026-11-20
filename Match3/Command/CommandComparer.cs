using System;
using System.Collections.Generic;

namespace Match3.Command;

public sealed class CommandComparer : IEqualityComparer<ICommand>
{
    public bool Equals(ICommand x, ICommand y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Key == y.Key;
    }

    public int GetHashCode(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        return command.Key.GetHashCode();
    }
}