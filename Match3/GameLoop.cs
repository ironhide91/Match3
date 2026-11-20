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
                    // failed to parse, keep reading for a valid command text
                    continue;
                }

                previous = active;

                // automatically moves down by one row at end of the frame
                // so we evaluate taking that into consideration
                active = active.Offset(row: 1, column: 0);

                // generate change set for clearing previous brick
                // apply the chaneg set
                var spawnBrickChangeSet = ClearSelfChangeSetGenerator.Instatnce.Generate(previous);
                board.Apply(spawnBrickChangeSet);

                // iteratte ove each command
                foreach (var command in commands)
                {
                    // generate change set for this command
                    var changeSet = command.Generator.Generate(board, active);

                    // validate change set
                    if (!board.Validate(changeSet))
                    {
                        // failed validation, ignore this command
                        continue;
                    }

                    // derive target brcik from change set
                    // this it the new active brick
                    active = new Brick(
                        active.Orientation,
                        changeSet.Writes.Select(x => new Cell(new Position(x.Row, x.Column), x.Symbol)).ToList());

                    // apply change set
                    _session.Board.Apply(changeSet);
                }

                // once all commnds are applied, we check whether it is settled
                if (_session.Board.IsConnected(active))
                {
                    // commit settled brick
                    board.Settle(active);

                    // signal to exit loop, so the next brick can be fetched
                    settled = true;

                    // good time to identify matching cells
                    var foundMatches = _matchFinder.TryFindMatches(
                        minMatchLength: 3,
                        allowedSymbols: _config.GameRules.AllowedSymbols,
                        board: board,
                        matches: out var matches);

                    if (foundMatches)
                    {
                        // matches found, generate change set to clear them
                        var matchesChangeSet = ClearMatchesChangeSetGenerator.Instatnce.Generate(
                            matches, _config.EmptyCellSymbol);

                        // clear the matches by applying change set
                        board.Apply(matchesChangeSet);

                        // unsettle matches
                        var positions = matchesChangeSet.Writes
                            .Select(x => new Position(x.Row, x.Column))
                            .ToHashSet();
                        board.UnSettleMatches(positions);
                    }
                }

                // render frame once per set of commands
                _frameRenderer.Render(++frameNumber, board);
            }
        }
    }
}
