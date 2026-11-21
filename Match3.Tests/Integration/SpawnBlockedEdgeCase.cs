using AwesomeAssertions;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void SpawnBlockedEdgeCase_WhenBoardHeightIsFilled_AssertsEachFrameAndEndsImmediately()
    {
        // Arrange: 3x3 board with two vertical bricks of length 3; second brick cannot spawn
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "3 3 V*@^ V^*@",
            "D", // Frame 1 -> Frame 2 (drops first vertical brick to rows 0..2 and settles it)
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert
        frames.Should().HaveCount(2);

        // Frame 1: V*@^ spawned centered in col 1
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            ".*.",
            ".@.",
            ".^."
        ]).Should().BeTrue();

        // Frame 2: Brick 1 settled; Brick 2 cannot spawn because (0,1),(1,1),(2,1) are occupied -> Game Over
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            ".*.",
            ".@.",
            ".^."
        ]).Should().BeTrue();

        output.Should().Contain("Game Over.");
        output.Should().Contain("Thank you for playing Match-3!");
    }
}