using Match3.BrickProvider;
using Match3.Canvas;

namespace Match3;

public sealed class GameSession
{
    private readonly IBrickSourceProvider _brickSourceProvider;

    private readonly Board _board;

    public GameSession(
        int boardHeight,
        int boardWidth,
        char emptySymbol,
        IBrickSourceProvider brickSourceProvider)
    {
        _brickSourceProvider = brickSourceProvider;
        _board = new Board(boardHeight, boardWidth, emptySymbol);
    }

    public IBrickSourceProvider BrickSourceProvider => _brickSourceProvider;

    internal Board Board => _board;

    public void Replay()
    {

    }

    public void Undo()
    {

    }
}
