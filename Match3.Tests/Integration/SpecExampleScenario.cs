using AwesomeAssertions;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void SpecExampleScenario_ExecutesAllEightFramesAndClearsBottomMatchBeforeGameOver()
    {
        // Arrange: Exact scenario from Match-3 Take-Home Assignment (v3)
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "5 8 H^^* V*@^", // Setup
            "LL",           // Frame 1 -> Frame 2
            "R",            // Frame 2 -> Frame 3
            "DR",           // Frame 3 -> Frame 4
            "LLR",          // Frame 4 -> Frame 5
            "",             // Frame 5 -> Frame 6
            "R",            // Frame 6 -> Frame 7
            "DR",           // Frame 7 -> Frame 8
            "Q"             // Quit
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert: Exact total frame count and every single frame's board state
        frames.Should().HaveCount(8);

        // Frame 1: Initial spawn of H^^* centered in top row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            ".^^*.",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            "....."
        ]).Should().BeTrue();

        // Frame 2: First 'L' shifts left to col 0; second 'L' ignored (wall); auto-drops 1 row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            ".....",
            "^^*..",
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            "....."
        ]).Should().BeTrue();

        // Frame 3: 'R' shifts right to col 1; auto-drops 1 row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 3,
        [
            ".....",
            ".....",
            ".^^*.",
            ".....",
            ".....",
            ".....",
            ".....",
            "....."
        ]).Should().BeTrue();

        // Frame 4: 'D' drops to bottom row 7, 'R' shifts to cols 2..4, Brick 1 settles; Brick 2 (V*@^) spawns
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 4,
        [
            "..*..",
            "..@..",
            "..^..",
            ".....",
            ".....",
            ".....",
            ".....",
            "..^^*"
        ]).Should().BeTrue();

        // Frame 5: 'LLR' -> only first 2 ('LL') processed; 1st 'L' moves to col 1, 2nd 'L' moves to col 0; auto-drops 1 row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 5,
        [
            ".....",
            "*....",
            "@....",
            "^....",
            ".....",
            ".....",
            ".....",
            "..^^*"
        ]).Should().BeTrue();

        // Frame 6: Blank input -> auto-drops 1 row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 6,
        [
            ".....",
            ".....",
            "*....",
            "@....",
            "^....",
            ".....",
            ".....",
            "..^^*"
        ]).Should().BeTrue();

        // Frame 7: 'R' shifts right to col 1; auto-drops 1 row
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 7,
        [
            ".....",
            ".....",
            ".....",
            ".*...",
            ".@...",
            ".^...",
            ".....",
            "..^^*"
        ]).Should().BeTrue();

        // Frame 8: 'D' drops to rows 5..7, 'R' blocked by settled '^' at (7,2), settles, clears ^^^ match on row 7
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 8,
        [
            ".....",
            ".....",
            ".....",
            ".....",
            ".....",
            ".*...",
            ".@...",
            "....*"
        ]).Should().BeTrue();

        output.Should().Contain("Welcome to Match-3 game!");
        output.Should().Contain("Game Over.");
        output.Should().Contain("Thank you for playing Match-3!");
    }
}