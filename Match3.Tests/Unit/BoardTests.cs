using AwesomeAssertions;
using Match3.Canvas;

namespace Match3.Tests.Unit;

public sealed class BoardTests
{
    [Theory]
    [InlineData(0, 5)]
    [InlineData(-1, 5)]
    [InlineData(5, 0)]
    [InlineData(5, -3)]
    public void Constructor_WhenDimensionsAreZeroOrNegative_ThrowsArgumentOutOfRangeException(int width, int height)
    {
        // Arrange & Act
        var act = () => new Board(width, height, '.');

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WhenDimensionsArePositive_InitializesAllCellsWithEmptySymbol()
    {
        // Arrange
        const int width = 4;
        const int height = 3;

        // Act
        var board = new Board(width, height, '.');

        // Assert
        board.Width.Should().Be(width);
        board.Height.Should().Be(height);
        board.SettledCells.Should().BeEmpty();
        for (var r = 0; r < height; r++)
            for (var c = 0; c < width; c++)
                board.GetSymbol(r, c).Should().Be('.');
    }

    [Fact]
    public void CanApply_WhenChangeSetIsEmpty_ReturnsFalse()
    {
        // Arrange
        var board = new Board(5, 8, '.');

        // Act
        var result = board.CanApply(ChangeSet.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanApply_WhenDestinationIsOutOfBounds_ReturnsFalse()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(0, 0), '^'),
            new Cell(new Position(0, 1), '^'),
            new Cell(new Position(0, 2), '*')
        ]);
        var moveLeftOutOfBounds = ChangeSet.FromMove(brick.Cells, rowDelta: 0, colDelta: -1, '.');

        // Act
        var result = board.CanApply(moveLeftOutOfBounds);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanApply_WhenBrickOverlapsItsOwnClearedPositions_ReturnsTrue()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(0, 1), '^'),
            new Cell(new Position(0, 2), '^'),
            new Cell(new Position(0, 3), '*')
        ]);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));

        var moveRight = ChangeSet.FromMove(brick.Cells, rowDelta: 0, colDelta: 1, '.');

        // Act
        var result = board.CanApply(moveRight);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanApply_WhenDestinationIsBlockedByAnotherBrick_ReturnsFalse()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        board.Apply(new ChangeSet([], [new CellChange(new Position(1, 2), '@')]));

        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(0, 1), '^'),
            new Cell(new Position(0, 2), '^'),
            new Cell(new Position(0, 3), '*')
        ]);
        var moveDown = ChangeSet.FromMove(brick.Cells, rowDelta: 1, colDelta: 0, '.');

        // Act
        var result = board.CanApply(moveDown);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SettleAndClearPositions_UpdatesSettledTrackingAndGridSymbols()
    {
        // Arrange
        var board = new Board(5, 8, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(7, 1), '^'),
            new Cell(new Position(7, 2), '^'),
            new Cell(new Position(7, 3), '^')
        ]);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));

        // Act
        board.Settle(brick);
        board.ClearPositions([new Position(7, 1), new Position(7, 2), new Position(7, 3)]);

        // Assert
        board.SettledCells.Should().BeEmpty();
        board.GetSymbol(7, 1).Should().Be('.');
        board.GetSymbol(7, 2).Should().Be('.');
        board.GetSymbol(7, 3).Should().Be('.');
    }

    [Fact]
    public void CaptureAndRestoreSnapshot_RestoresExactGridAndSettledCells()
    {
        // Arrange
        var board = new Board(4, 4, '.');
        var brick = new Brick(Orientation.Horizontal,
        [
            new Cell(new Position(3, 0), '*'),
            new Cell(new Position(3, 1), '@'),
            new Cell(new Position(3, 2), '^')
        ]);
        board.Apply(new ChangeSet([], brick.Cells.Select(c => new CellChange(c.Position, c.Symbol)).ToArray()));
        board.Settle(brick);
        var snapshot = board.CaptureSnapshot();

        // Mutate board after snapshot
        board.ClearPositions([new Position(3, 0), new Position(3, 1), new Position(3, 2)]);

        // Act
        board.RestoreSnapshot(snapshot);

        // Assert
        board.GetSymbol(3, 0).Should().Be('*');
        board.GetSymbol(3, 1).Should().Be('@');
        board.GetSymbol(3, 2).Should().Be('^');
        board.SettledCells.Should().HaveCount(3);
    }
}