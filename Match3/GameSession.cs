using Match3.Canvas;
using Match3.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3;

public sealed class GameSession
{
    private readonly BoardSetup _setup;
    private readonly GameRules _rules;
    private readonly Stack<CommandRecord> _undoStack = new();
    private readonly List<CommandRecord> _history = [];

    public Board Board { get; }
    public int CurrentBrickIndex { get; private set; }
    public Brick ActiveBrick { get; private set; }
    public IReadOnlyList<CommandRecord> History => _history;

    public GameSession(BoardSetup setup, GameRules rules)
    {
        _setup = setup;
        _rules = rules;
        Board = new Board(setup.Width, setup.Height, rules.EmptyCellSymbol);
    }

    public bool TrySpawnNextBrick()
    {
        if (CurrentBrickIndex >= _setup.Bricks.Count)
        {
            ActiveBrick = null;
            return false;
        }

        var spawned = _setup.Bricks[CurrentBrickIndex].PlaceAtSpawn(Board.Width);
        var spawnChange = new ChangeSet([], spawned.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray());

        if (!Board.CanApply(spawnChange))
        {
            ActiveBrick = null;
            return false;
        }

        Board.Apply(spawnChange);
        ActiveBrick = spawned;
        CurrentBrickIndex++;
        return true;
    }

    public bool TryExecuteSingleCommand(IBrickCommand command)
    {
        if (ActiveBrick is null)
            return false;

        var beforeSnapshot = Board.CaptureSnapshot();
        var brickBefore = ActiveBrick;

        var plan = command.PlanMove(Board, ActiveBrick);
        if (Board.CanApply(plan))
        {
            Board.Apply(plan);
            ActiveBrick = new Brick(
                ActiveBrick.Orientation,
                plan.Writes.Select(w => new Cell(w.Position, w.Symbol)).ToArray());
        }

        var afterSnapshot = Board.CaptureSnapshot();
        var record = new CommandRecord(
            CurrentBrickIndex - 1,
            brickBefore,
            ActiveBrick,
            command,
            beforeSnapshot,
            afterSnapshot);

        _undoStack.Push(record);
        _history.Add(record);
        return true;
    }

    public void ApplyGravityAndSettleIfBlocked()
    {
        if (ActiveBrick is null)
            return;

        // Automatic 1-row gravity drop at the end of every frame
        if (Board.CanMoveDown(ActiveBrick))
        {
            var fallPlan = ChangeSet.FromMove(ActiveBrick.Cells, rowDelta: 1, colDelta: 0, Board.EmptySymbol);
            Board.Apply(fallPlan);
            ActiveBrick = ActiveBrick.Offset(rowDelta: 1, colDelta: 0);
        }

        // If the brick cannot move down any further, it becomes stationary
        if (!Board.CanMoveDown(ActiveBrick))
        {
            Board.Settle(ActiveBrick);
            ActiveBrick = null;

            // Spec: remove any horizontal/vertical (and diagonal) runs of 3+; no gravity applied
            var matches = MatchFinder.FindMatches(Board, _rules);
            if (matches.Count > 0)
                Board.ClearPositions(matches);
        }
    }

    /// <summary>
    /// Reverts the most recently executed single command and restores board/brick state.
    /// </summary>
    public bool UndoSingleCommand()
    {
        if (!_undoStack.TryPop(out var last))
            return false;

        Board.RestoreSnapshot(last.BoardBefore);
        ActiveBrick = last.ActiveBrickBefore;
        CurrentBrickIndex = last.BrickIndex + 1;
        return true;
    }

    /// <summary>
    /// Replays all recorded commands from the initial board setup, invoking a callback after each step.
    /// </summary>
    public void Replay(Action<int, Board, CommandRecord> onStepRendered)
    {
        Board.RestoreSnapshot(new Board(Board.Width, Board.Height, Board.EmptySymbol).CaptureSnapshot());
        CurrentBrickIndex = 0;
        ActiveBrick = null;

        var frame = 0;
        var commandsByBrick = _history.GroupBy(h => h.BrickIndex).ToDictionary(g => g.Key, g => g.ToList());

        while (TrySpawnNextBrick())
        {
            onStepRendered(++frame, Board, null);
            var brickIdx = CurrentBrickIndex - 1;

            if (!commandsByBrick.TryGetValue(brickIdx, out var brickCommands))
                break;

            foreach (var step in brickCommands)
            {
                if (ActiveBrick is null)
                    break;

                var plan = step.Command.PlanMove(Board, ActiveBrick);
                if (Board.CanApply(plan))
                {
                    Board.Apply(plan);
                    ActiveBrick = new Brick(
                        ActiveBrick.Orientation,
                        plan.Writes.Select(w => new Cell(w.Position, w.Symbol)).ToArray());
                }

                onStepRendered(++frame, Board, step);
            }

            if (ActiveBrick is not null)
                ApplyGravityAndSettleIfBlocked();
        }
    }
}
