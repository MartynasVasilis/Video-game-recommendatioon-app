namespace GamePicker.Domain.Recommendations;

public sealed record FilterOptions(
    IReadOnlyList<string> Platforms,
    IReadOnlyList<string> Genres,
    IReadOnlyList<string> Developers,
    IReadOnlyList<string> Dimensions,
    IReadOnlyList<int> ReleaseYears)
{
    public static FilterOptions From(IEnumerable<Game> catalog)
    {
        var games = catalog.ToList();

        return new FilterOptions(
            Sorted(games.SelectMany(game => game.Platforms)),
            Sorted(games.Select(game => game.Genre)),
            Sorted(games.Select(game => game.Developer)),
            Sorted(games.Select(game => game.Dimension)),
            Sorted(games.Select(game => game.ReleaseYear)));
    }

    private static List<T> Sorted<T>(IEnumerable<T> values) => values.Distinct().Order().ToList();
}
