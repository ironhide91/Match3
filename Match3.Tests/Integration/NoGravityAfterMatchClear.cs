using AwesomeAssertions;
using Xunit;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void NoGravityAfterMatchClear_WhenBottomCellsAreCleared_AssertsEachFrame()
    {
        // Arrange: Brick 1 (H^^^) clears at row 5; Brick 2 (V*@*) drops to row 5
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "5 6 H^^^ V*@*",
            "D", // Frame 1 -> Frame 2 (H^^^ settles at row 5 and clears immediately; V*@* spawns)
            "D", // Frame 2 -> Frame 3 (V*@* settles at rows 3..5)
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert
        frames.Should().HaveCount(3);

        // Frame 1: H^^^ spawned centered at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            ".^^^.",
            ".....",
            ".....",
            ".....",
            ".....",
            "....."
        ]).Should().BeTrue();

        // Frame 2: H^^^ dropped, settled, and cleared; V*@* spawned at rows 0..2
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            "..*..",
            "..@..",
            "..*..",
            ".....",
            ".....",
            "....."
        ]).Should().BeTrue();

        // Frame 3: V*@* dropped and settled at rows 3..5
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 3,
        [
            ".....",
            ".....",
            ".....",
            "..*..",
            "..@..",
            "..*.."
        ]).Should().BeTrue();

        output.Should().Contain("Game Over.");
    }
}