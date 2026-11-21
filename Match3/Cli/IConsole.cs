namespace Match3.Cli;

public interface IRead
{
    string ReadLine();
}

public interface IWrite
{
    void WriteLine(string text);
}
public interface IConsole : IRead, IWrite
{

}
