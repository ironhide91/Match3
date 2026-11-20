using System;

namespace Match3.Cli;

/// <summary>
/// 
/// </summary>
public class SystemConsole : IConsole
{
    public string ReadLine()
    {
        return Console.ReadLine();
    }

    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }
}
