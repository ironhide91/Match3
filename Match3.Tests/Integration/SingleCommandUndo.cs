using AwesomeAssertions;
using Match3.Canvas;
using Match3.Commands;
using Match3.Parsers;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void SingleCommandUndo_WhenExecutedMidFrame_RevertsExactSingleCommandState()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseSetup("5 8 H^^*", rules, out var setup);
        var session = new GameSession(setup, rules);
        session.TrySpawnNextBrick(); // cols 1, 2, 3

        // Act: execute L (cols 0, 1, 2), then R (cols 1, 2, 3), then undo R
        session.TryExecuteSingleCommand(new LeftCommand());
        session.TryExecuteSingleCommand(new RightCommand());
        var undone = session.UndoSingleCommand();

        // Assert
        undone.Should().BeTrue();
        session.ActiveBrick.Cells.Select(c => c.Position)
            .Should()
            .Equal(
                new Position(0, 0),
                new Position(0, 1),
                new Position(0, 2));
        session.Board.GetSymbol(0, 0).Should().Be('^');
        session.Board.GetSymbol(0, 3).Should().Be('.');
    }
}