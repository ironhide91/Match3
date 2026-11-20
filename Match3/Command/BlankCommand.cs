namespace Match3.Command;

public class BlankCommand : ICommand
{
    public static readonly ICommand Instance = new BlankCommand();

    public char Key => ' ';

    public string Desc => "Move dow by one position";

    private BlankCommand()
    {

    }
}