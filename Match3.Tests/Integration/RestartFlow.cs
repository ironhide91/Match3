using AwesomeAssertions;

namespace Match3.Tests.Integration;

public sealed partial class Match3IntegrationTests
{
    [Fact]
    public void RestartFlow_WhenUserEntersSAtGameOver_AssertsEachFrameAcrossBothSessions()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();
        string[] inputs =
        [
            "3 4 H*@^",
            "D",
            "S",          // Restart
            "4 4 V^*@",
            "D",
            "Q"
        ];

        // Act
        var output = TestFixtures.RunConsoleSession(rules, inputs);
        var sessions = output.Split("Game Over.", StringSplitOptions.RemoveEmptyEntries);

        // Assert
        sessions.Should().HaveCount(3); // Session 1, Session 2, and Quit prompt
        var session1Output = sessions[0];
        var session2Output = sessions[1];

        // Session 1: Frame 1 & Frame 2
        FrameAssertionHelper.ExtractRenderedFrames(session1Output).Should().HaveCount(2);
        FrameAssertionHelper.IsFrameMatch(session1Output, frameNumber: 1,
        [
            "*@^",
            "...",
            "...",
            "..."
        ]).Should().BeTrue();

        FrameAssertionHelper.IsFrameMatch(session1Output, frameNumber: 2,
        [
            "...",
            "...",
            "...",
            "*@^"
        ]).Should().BeTrue();

        // Session 2: Frame 1 & Frame 2
        FrameAssertionHelper.ExtractRenderedFrames(session2Output).Should().HaveCount(2);
        FrameAssertionHelper.IsFrameMatch(session2Output, frameNumber: 1,
        [
            ".^..",
            ".*..",
            ".@..",
            "...."
        ]).Should().BeTrue();

        FrameAssertionHelper.IsFrameMatch(session2Output, frameNumber: 2,
        [
            "....",
            ".^..",
            ".*..",
            ".@.."
        ]).Should().BeTrue();

        output.Should().Contain("Thank you for playing Match-3!");
    }
}