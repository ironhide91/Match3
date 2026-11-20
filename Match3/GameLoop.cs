using Match3.Canvas;
using Match3.ChangeSetGenerator;
using Match3.Cli;
using Match3.Match;
using Match3.Parser;
using System.Linq;

namespace Match3;

public class GameLoop
{
    private readonly GameConfig _config;
    private readonly GameSession _session;
    private readonly IMatchFinder _matchFinder;
    private readonly IRead _reader;
    private readonly FrameRenderer _frameRenderer;

    public GameLoop(
        GameConfig config,
        GameSession session,
        IMatchFinder matchFinder,
        IRead reader,
        FrameRenderer frameRenderer)
    {
        _config = config;
        _session = session;
        _matchFinder = matchFinder;
        _reader = reader;
        _frameRenderer = frameRenderer;
    }

    public GameLoopResult Run()
    {
        var board = _session.Board;
        var frameNumber = 0;

        while (true)
        {
            if (!_session.BrickSourceProvider.TryNext(out var nextBrick))
            {
                return GameLoopResult.RunOutOfBricks;
            }

            var spawnBrick = SpawnPositioner.PlaceAtSpawn(nextBrick, board.Width);
            var spawnChangeSet = SpawnChangeSetGenerator.Generate(spawnBrick);

            if (!board.Validate(spawnChangeSet))
            {
                return GameLoopResult.RunOutOfSpace;
            }

            board.Apply(spawnChangeSet);
            _frameRenderer.Render(++frameNumber, board);

            var settled = false;

            Brick previous = null;
            Brick active = spawnBrick;

            while (!settled)
            {
                if (!CommandsParser.TryParse(
                        _reader.ReadLine(),
                        _config.GameRules.CommandsPerFrame,
                        _config.CommandRegistry,
                        out var commands))
                {
                    continue;
                }

                previous = active;
                active = active.Offset(1, 0);

                var spawnBrickChangeSet = ClearSelfChangeSetGenerator.Instatnce.Generate(previous);
                board.Apply(spawnBrickChangeSet);

                foreach (var command in commands)
                {
                    var changeSet = command.Generator.Generate(board, active);

                    if (!board.Validate(changeSet))
                    {
                        continue;
                    }

                    active = new Brick(
                        active.Orientation,
                        changeSet.Writes.Select(x => new Cell(new Position(x.Row, x.Column), x.Symbol)).ToList());

                    _session.Board.Apply(changeSet);
                }

                if (_session.Board.IsConnected(active))
                {
                    board.Settle(active);
                    settled = true;

                    var foundMatches = _matchFinder.TryFindMatches(
                        minMatchLength: 3,
                        allowedSymbols: _config.GameRules.AllowedSymbols,
                        board: board,
                        matches: out var matches);

                    if (foundMatches)
                    {
                        var matchesChangeSet = ClearMatchesChangeSetGenerator.Instatnce.Generate(
                            matches, _config.EmptyCellSymbol);

                        board.Apply(matchesChangeSet);
                    }

                    //break;
                }

                _frameRenderer.Render(++frameNumber, board);
            }
        }
    }
}
