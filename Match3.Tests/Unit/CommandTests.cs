using AwesomeAssertions;
using Match3.Canvas;
using Match3.Commands;
using Match3.Parsers;

namespace Match3.Tests.Unit;

public sealed class CommandTests
{
    [Theory]
    [InlineData(5, 1)] // (5 - 3) / 2 = 1 -> cols 1, 2, 3
    [InlineData(6, 1)] // (6 - 3) / 2 = 1 -> cols 1, 2, 3
    [InlineData(7, 2)] // (7 - 3) / 2 = 2 -> cols 2, 3, 4
    public void PlaceAtSpawn_ForHorizontalBrick_CentersInTopRow(int boardWidth, int expectedStartCol)
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseBrick("H^^*", rules, out var brick);

        // Act
        var spawned = brick.PlaceAtSpawn(boardWidth);

        // Assert
        spawned.Cells.Select(c => c.Position)
            .Should().Equal(
                new Position(0, expectedStartCol),
                new Position(0, expectedStartCol + 1),
                new Position(0, expectedStartCol + 2));
    }

    [Theory]
    [InlineData(5, 2)] // (5 - 1) / 2 = 2
    [InlineData(4, 1)] // (4 - 1) / 2 = 1
    public void PlaceAtSpawn_ForVerticalBrick_CentersInTopThreeRows(int boardWidth, int expectedCol)
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseBrick("V*@^", rules, out var brick);

        // Act
        var spawned = brick.PlaceAtSpawn(boardWidth);

        // Assert
        spawned.Cells.Select(c => c.Position)
            .Should().Equal(
                new Position(0, expectedCol),
                new Position(1, expectedCol),
                new Position(2, expectedCol));
    }

    [Fact]
    public void PlaceAtSpawn_ForDiagonalUpRightBrick_NormalizesMinRowToZeroAndCentersHorizontally()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseBrick("/@*^", rules, out var brick);

        // Act
        var spawned = brick.PlaceAtSpawn(boardWidth: 5);

        // Assert
        spawned.Cells.Select(c => c.Position)
            .Should().Equal(
                new Position(2, 1),
                new Position(1, 2),
                new Position(0, 3));
    }

    [Fact]
    public void LeftAndRightCommands_PlanSingleColumnShift()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(0, 1), '^'),
            new Cell(new Position(0, 2), '^'),
            new Cell(new Position(0, 3), '*')
        ]);

        // Act
        var leftPlan = new LeftCommand().PlanMove(board, brick);
        var rightPlan = new RightCommand().PlanMove(board, brick);

        // Assert
        leftPlan.Writes.Select(w => w.Position)
            .Should().Equal(new Position(0, 0), new Position(0, 1), new Position(0, 2));
        rightPlan.Writes.Select(w => w.Position)
            .Should().Equal(new Position(0, 2), new Position(0, 3), new Position(0, 4));
    }

    [Fact]
    public void DropCommand_WhenObstacleExistsInOneColumn_StopsAboveHighestObstacle()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        board.Apply(new ChangeSet([], [new CellChange(new Position(5, 3), '@')]));

        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(0, 1), '^'),
            new Cell(new Position(0, 2), '^'),
            new Cell(new Position(0, 3), '*')
        ]);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));

        // Act
        var dropPlan = new DropCommand().PlanMove(board, brick);

        // Assert
        dropPlan.Writes.Select(w => w.Position)
            .Should().Equal(new Position(4, 1), new Position(4, 2), new Position(4, 3));
    }

    [Fact]
    public void DropCommand_ForDiagonalBrick_DropsUntilLowestCornerTouchesFloor()
    {
        // Arrange
        var board = new Board(5, 6, '.');
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseBrick(@"\^*@", rules, out var rawBrick);
        var brick = rawBrick.PlaceAtSpawn(5);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));

        // Act
        var dropPlan = new DropCommand().PlanMove(board, brick);

        // Assert
        dropPlan.Writes.Select(w => w.Position)
            .Should().Equal(new Position(3, 1), new Position(4, 2), new Position(5, 3));
    }
}
