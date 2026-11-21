using AwesomeAssertions;
using Match3.Commands;
using Match3.Parsers;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void Replay_ReconstructsCompleteCommandSequenceFromInitialBoard()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        InputParser.TryParseSetup("5 8 H^^* V*@^", rules, out var setup);
        var session = new GameSession(setup, rules);

        session.TrySpawnNextBrick();
        session.TryExecuteSingleCommand(new LeftCommand());
        session.TryExecuteSingleCommand(new DropCommand());
        session.ApplyGravityAndSettleIfBlocked();

        var recordedSteps = new List<char?>();

        // Act
        session.Replay((_, _, record) => recordedSteps.Add(record?.Command.Key));

        // Assert
        recordedSteps.Should().ContainInOrder(null, 'L', 'D');
        session.Board.SettledCells.Should().HaveCount(3);
        session.Board.GetSymbol(7, 0).Should().Be('^');
        session.Board.GetSymbol(7, 1).Should().Be('^');
        session.Board.GetSymbol(7, 2).Should().Be('*');
    }
}