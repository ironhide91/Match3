namespace Match3;

public record class GameConfig(
    char EmptyCellSymbol,
    GameRules GameRules,
    CommandRegistry CommandRegistry);