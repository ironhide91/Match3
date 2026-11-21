using AwesomeAssertions;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Theory]
    [InlineData(3, 4, 2, "H*^ V@*")]
    [InlineData(7, 10, 4, "H*@^@ V^@*@")]
    [InlineData(6, 7, 3, @"\*@^ /*@^")]
    public void BoardAndBrickVariants_WithDifferentDimensionsBrickSizesAndDiagonals_AssertsFrameProgression(
        int width,
        int height,
        int brickLength,
        string bricksInput)
    {
        // Arrange
        var rules = TestFixtures.DefaultRules(brickLength: brickLength);
        string[] inputs =
        [
            $"{width} {height} {bricksInput}",
            "D",
            "D",
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert: 2 bricks dropped immediately -> Frame 1 (spawn 1), Frame 2 (spawn 2), Frame 3 (final settled)
        frames.Should().HaveCount(3);
        foreach (var frame in frames.Values)
        {
            frame.Should().HaveCount(height);
            frame.Should().OnlyContain(row => row.Length == width);
        }

        output.Should().Contain("Game Over.");
        output.Should().Contain("Thank you for playing Match-3!");
    }
}