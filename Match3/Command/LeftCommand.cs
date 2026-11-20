namespace Match3.Command;

public class LeftCommand : ICommand
{
    public static readonly ICommand Instance = new LeftCommand();

    public char Key => 'L';

    public string Desc => "Move left by one position";

    private LeftCommand()
    {
        
    }    
}
