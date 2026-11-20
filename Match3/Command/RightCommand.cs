namespace Match3.Command;

public class RightCommand : ICommand
{
    public static readonly ICommand Instance = new RightCommand();

    public char Key => 'R';

    public string Desc => "Move right by one position";

    private RightCommand()
    {

    }
}
