using AwesomeAssertions;
using Xunit;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void StackedBricksUntilSpawnBlocked_WhenBricksStackToTopAndFinalBrickHasNoSpace_AssertsEachFrameAndEndsGame()
    {
        // Arrange: 3x4 board with four horizontal bricks of length 3.
        // Bricks 1..4 stack from row 3 up to row 0; after Brick 4 settles at row 0, no further brick can spawn -> Game Over.
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "3 4 H*@^ H@^* H@^* H*@^",
            "D", // Frame 1 -> Frame 2 (Brick 1 settles at row 3; Brick 2 spawns at row 0)
            "D", // Frame 2 -> Frame 3 (Brick 2 settles at row 2; Brick 3 spawns at row 0)
            "D", // Frame 3 -> Frame 4 (Brick 3 settles at row 1; Brick 4 spawns at row 0)
            "D", // Frame 4 -> Frame 5 (Brick 4 settles at row 0; next spawn blocked -> Game Over)
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert
        frames.Should().HaveCount(5);

        // Frame 1: Brick 1 (*@^) spawned at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            "*@^",
            "...",
            "...",
            "..."
        ]).Should().BeTrue();

        // Frame 2: Brick 1 settled at row 3; Brick 2 (@^*) spawned at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            "@^*",
            "...",
            "...",
            "*@^"
        ]).Should().BeTrue();

        // Frame 3: Brick 2 settled at row 2; Brick 3 (@^*) spawned at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 3,
        [
            "@^*",
            "...",
            "@^*",
            "*@^"
        ]).Should().BeTrue();

        // Frame 4: Brick 3 settled at row 1; Brick 4 (*@^) spawned at row 0
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 4,
        [
            "*@^",
            "@^*",
            "@^*",
            "*@^"
        ]).Should().BeTrue();

        // Frame 5: Brick 4 settled at row 0; board is full and no further brick can spawn -> Game Over
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 5,
        [
            "*@^",
            "@^*",
            "@^*",
            "*@^"
        ]).Should().BeTrue();

        output.Should().Contain("Game Over.");
        output.Should().Contain("Thank you for playing Match-3!");
    }
}