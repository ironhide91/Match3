using AwesomeAssertions;
using Match3.Canvas;
using Match3.Parsers;

namespace Match3.Tests.Unit;

public sealed class InputParserTests
{
    [Fact]
    public void TryParseSetup_WhenValidSetupWithHorizontalAndVerticalBricks_ParsesBoardAndBricks()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();

        // Act
        var parsed = InputParser.TryParseSetup("5 8 H^^* V*@^", rules, out var setup);

        // Assert
        parsed.Should().BeTrue();
        setup.Width.Should().Be(5);
        setup.Height.Should().Be(8);
        setup.Bricks.Should().HaveCount(2);
        setup.Bricks[0].Orientation.Should().Be(Orientation.Horizontal);
        setup.Bricks[1].Orientation.Should().Be(Orientation.Vertical);
    }

    [Fact]
    public void TryParseSetup_WhenDiagonalOrientationsAreProvided_ParsesDiagonalDownRightAndUpRight()
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();

        // Act
        var parsed = InputParser.TryParseSetup(@"6 10 \^*@ /@*^", rules, out var setup);

        // Assert
        parsed.Should().BeTrue();
        setup.Bricks[0].Orientation.Should().Be(Orientation.DiagonalDownRight);
        setup.Bricks[0].Cells.Select(c => c.Position)
            .Should().Equal(new Position(0, 0), new Position(1, 1), new Position(2, 2));

        setup.Bricks[1].Orientation.Should().Be(Orientation.DiagonalUpRight);
        setup.Bricks[1].Cells.Select(c => c.Position)
            .Should().Equal(new Position(0, 0), new Position(-1, 1), new Position(-2, 2));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("5 8")]                                // Missing bricks
    [InlineData("0 8 H^^*")]                           // Zero width
    [InlineData("5 -2 H^^*")]                          // Negative height
    [InlineData("abc 8 H^^*")]                         // Non-numeric dimension
    [InlineData("5 8 X^^*")]                           // Invalid orientation
    [InlineData("5 8 H^^")]                            // Too short brick
    [InlineData("5 8 H^^**")]                          // Too long brick
    [InlineData("5 8 H^#*")]                           // Disallowed symbol '#'
    [InlineData("5 8 H^^.")]                           // Empty cell symbol '.' inside brick
    [InlineData("5 8 H^^* V*@^ H*** V@@@ H^^^ V***")]  // Exceeds MaxBricks (5)
    public void TryParseSetup_WhenInputViolatesSpec_ReturnsFalse(string? input)
    {
        // Arrange
        var rules = TestFixtures.DefaultRules();

        // Act
        var parsed = InputParser.TryParseSetup(input, rules, out _);

        // Assert
        parsed.Should().BeFalse();
    }

    [Fact]
    public void ParseFrameCommands_WhenMoreThanTwoValidCommandsAreEntered_TakesOnlyFirstTwo()
    {
        // Arrange
        var registry = TestFixtures.DefaultRegistry();

        // Act
        var commands = InputParser.ParseFrameCommands("LLR", maxCommands: 2, registry);

        // Assert
        commands.Should().HaveCount(2);
        commands.Select(c => c.Key).Should().Equal('L', 'L');
    }

    [Fact]
    public void ParseFrameCommands_WhenInvalidCharactersAreMixedWithValidCommands_IgnoresInvalidAndKeepsValid()
    {
        // Arrange
        var registry = TestFixtures.DefaultRegistry();

        // Act
        var commands = InputParser.ParseFrameCommands("xR9zD!", maxCommands: 2, registry);

        // Assert
        commands.Select(c => c.Key).Should().Equal('R', 'D');
    }

    [Fact]
    public void ParseFrameCommands_WhenInputIsEmptyOrLowercase_HandlesBlankAndCaseInsensitivity()
    {
        // Arrange
        var registry = TestFixtures.DefaultRegistry();

        // Act
        var blankCommands = InputParser.ParseFrameCommands("", maxCommands: 2, registry);
        var lowerCommands = InputParser.ParseFrameCommands("ld", maxCommands: 2, registry);

        // Assert
        blankCommands.Should().ContainSingle().Which.Key.Should().Be(' ');
        lowerCommands.Select(c => c.Key).Should().Equal('L', 'D');
    }
}
