using AwesomeAssertions;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void DiagonalMatchClear_WhenThreeMatchingSymbolsAlignDiagonally_AssertsEachFrame()
    {
        // Arrange: 3x3 board with diagonal down-right \*** which immediately matches on settle
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            @"3 3 \***",
            "D", // Frame 1 -> Frame 2
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var frames = FrameAssertionHelper.ExtractRenderedFrames(output);

        // Assert
        frames.Should().HaveCount(2);

        // Frame 1: \*** occupies diagonal (0,0), (1,1), (2,2)
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 1,
        [
            "*..",
            ".*.",
            "..*"
        ]).Should().BeTrue();

        // Frame 2: Already touching bottom at (2,2), settles and clears diagonal match
        FrameAssertionHelper.IsFrameMatch(output, frameNumber: 2,
        [
            "...",
            "...",
            "..."
        ]).Should().BeTrue();
    }
}