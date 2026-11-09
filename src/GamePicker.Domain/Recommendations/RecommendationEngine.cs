namespace GamePicker.Domain.Recommendations;

public sealed class RecommendationEngine
{
    public const string NothingFoundMessage = "Rekomendacijų pagal duotus filtrus nerasta.";

    public RecommendationResult Recommend(
        RecommendationRequest request,
        IEnumerable<Game> catalog,
        IReadOnlyDictionary<int, LibraryStatus> library)
    {
        request.Validate(FilterOptions.From(catalog));

        var hardMatches = catalog.Where(game => request.Hard.Matches(game, library)).ToList();
        var candidates = hardMatches.Where(request.Auxiliary.Matches).ToList();
        IReadOnlyList<string> removedFilters = [];

        if (candidates.Count == 0)
        {
            candidates = hardMatches;
            removedFilters = request.Auxiliary.ActiveFilterNames();
        }

        if (candidates.Count == 0)
            return new RecommendationResult([], removedFilters, NothingFoundMessage);

        var ranked = Rank(candidates.Select(game => Score(game, request.Weights)));
        var games = LimitGenres(ranked, request.MaxPerGenre).Take(request.Count).ToList();

        return new RecommendationResult(games, removedFilters, null);
    }
    private static RecommendedGame Score(Game game, IReadOnlyDictionary<string, int> weights)
    {
        var breakdown = game.Tags.Concat(game.Moods)
            .Where(attribute => weights.GetValueOrDefault(attribute) > 0)
            .Select(attribute => new ScoreLine(attribute, weights[attribute]))
            .ToList();

        return new RecommendedGame(
            game.Id, game.Title, game.Genre, game.DurationHours, breakdown.Sum(line => line.Points), breakdown);
    }

    private static IEnumerable<RecommendedGame> Rank(IEnumerable<RecommendedGame> games) =>
        games.OrderByDescending(game => game.Score)
            .ThenByDescending(game => game.Breakdown.Count)
            .ThenBy(game => game.DurationHours)
            .ThenBy(game => game.Title, StringComparer.Ordinal)
            .ThenBy(game => game.GameId);

    private static IEnumerable<RecommendedGame> LimitGenres(IEnumerable<RecommendedGame> ranked, int? maxPerGenre)
    {
        var shownPerGenre = new Dictionary<string, int>();

        foreach (var game in ranked)
        {
            var shown = shownPerGenre.GetValueOrDefault(game.Genre);
            if (shown >= maxPerGenre)
                continue;

            shownPerGenre[game.Genre] = shown + 1;
            yield return game;
        }
    }
}
