namespace Match3.Command;

public class DropCommand : ICommand
{
    public static readonly ICommand Instance = new DropCommand();

    public char Key => 'D';

    public string Desc => "Move down";

    private DropCommand()
    {

    }
}
