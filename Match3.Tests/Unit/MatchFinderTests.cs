using AwesomeAssertions;
using Match3.Canvas;

namespace Match3.Tests.Unit;

public sealed class MatchFinderTests
{
    [Fact]
    public void FindMatches_WhenHorizontalRunOfThreeExists_ReturnsAllMatchedPositions()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        var board = new Board(5, 5, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(4, 1), '*'),
            new Cell(new Position(4, 2), '*'),
            new Cell(new Position(4, 3), '*')
        ]);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));
        board.Settle(brick);

        // Act
        var matches = MatchFinder.FindMatches(board, rules);

        // Assert
        matches.Should().BeEquivalentTo(
        [
            new Position(4, 1),
            new Position(4, 2),
            new Position(4, 3)
        ]);
    }

    [Fact]
    public void FindMatches_WhenVerticalRunOfFourExists_ReturnsAllFourPositions()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        var board = new Board(5, 6, '.');
        var cells = Enumerable.Range(2, 4).Select(r => new Cell(new Position(r, 2), '@')).ToArray();
        var brick = new Brick(Orientation.Vertical, cells);
        board.Apply(new ChangeSet([], cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));
        board.Settle(brick);

        // Act
        var matches = MatchFinder.FindMatches(board, rules);

        // Assert
        matches.Should().HaveCount(4);
    }

    [Fact]
    public void FindMatches_WhenDiagonalRunsExistInBothDirections_DetectsBothDiagonals()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        var board = new Board(5, 5, '.');

        var diagDown = new Brick(Orientation.DiagonalDownRight,
        [
            new Cell(new Position(0, 0), '^'),
            new Cell(new Position(1, 1), '^'),
            new Cell(new Position(2, 2), '^')
        ]);

        var diagUp = new Brick(Orientation.DiagonalUpRight,
        [
            new Cell(new Position(4, 2), '*'),
            new Cell(new Position(3, 3), '*'),
            new Cell(new Position(2, 4), '*')
        ]);

        board.Apply(new ChangeSet([], diagDown.Cells.Concat(diagUp.Cells)
            .Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));
        board.Settle(diagDown);
        board.Settle(diagUp);

        // Act
        var matches = MatchFinder.FindMatches(board, rules);

        // Assert
        matches.Should().HaveCount(6);
    }

    [Fact]
    public void FindMatches_WhenOverlappingHorizontalAndVerticalMatchesExist_ClearsAllSimultaneously()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        var board = new Board(5, 5, '.');
        var positions = new[]
        {
            new Position(4, 0), new Position(4, 1), new Position(4, 2),
            new Position(2, 2), new Position(3, 2)
        };
        var brick = new Brick(Orientation.Horizontal, positions.Select(p => new Cell(p, '^')).ToArray());
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));
        board.Settle(brick);

        // Act
        var matches = MatchFinder.FindMatches(board, rules);

        // Assert
        matches.Should().BeEquivalentTo(positions);
    }

    [Fact]
    public void FindMatches_WhenCellsAreNotSettledYet_DoesNotMatchActiveCells()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        var board = new Board(5, 5, '.');
        board.Apply(new ChangeSet([],
        [
            new CellChange(new Position(1, 1), '*'),
            new CellChange(new Position(1, 2), '*'),
            new CellChange(new Position(1, 3), '*')
        ]));

        // Act
        var matches = MatchFinder.FindMatches(board, rules);

        // Assert
        matches.Should().BeEmpty();
    }
}