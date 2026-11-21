using AwesomeAssertions;
using Xunit;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void FloatingMatchClear_WhenHorizontalMatchClearsAboveSettledCells_AssertsEveryFrame()
    {
        // Arrange: Brick 1 lands at bottom (row 4, cols 0..2); Brick 2 (H^^^) lands on top at row 3 and clears
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "4 5 H*@^ H^^^ V@*@",
            "LD", // Frame 1 -> Frame 2 (Brick 1 settles at row 4, cols 0..2; Brick 2 spawns)
            "LD", // Frame 2 -> Frame 3 (Brick 2 lands at row 3, cols 0..2 -> matches ^^^ and clears!; Brick 3 spawns)
            "RD", // Frame 3 -> Frame 4 (Brick 3 drops in col 3 -> settles at rows 2..4, col 3)
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert
        frames.Should().HaveCount(4);

        // Frame 1: H*@^ spawned at cols 0..2 (width 4, brick 3 -> (4-3)/2 = 0)
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            "*@^.",
            "....",
            "....",
            "....",
            "...."
        ]).Should().BeTrue();

        // Frame 2: Brick 1 settled on row 4; Brick 2 (H^^^) spawned at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            "^^^.",
            "....",
            "....",
            "....",
            "*@^."
        ]).Should().BeTrue();

        // Frame 3: Brick 2 landed on row 3, matched ^^^ and cleared; Brick 3 (V@*@) spawned at col 1
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 3,
        [
            ".@..",
            ".*..",
            ".@..",
            "....",
            "*@^."
        ]).Should().BeTrue();

        // Frame 4: Brick 3 dropped on right col 3; row 3 remains empty above row 4's settled *@^ cells
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 4,
        [
            "....",
            "..@.",
            "..*.",
            "..@.",
            "*@^."
        ]).Should().BeTrue();
    }
}