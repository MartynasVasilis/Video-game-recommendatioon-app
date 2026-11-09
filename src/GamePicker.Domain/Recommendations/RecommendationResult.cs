namespace GamePicker.Domain.Recommendations;

public sealed record ScoreLine(string Label, int Points);

public sealed record RecommendedGame(
    int GameId,
    string Title,
    string Genre,
    int DurationHours,
    int Score,
    IReadOnlyList<ScoreLine> Breakdown);

public sealed record RecommendationResult(
    IReadOnlyList<RecommendedGame> Games,
    IReadOnlyList<string> RemovedFilters,
    string? Message);
