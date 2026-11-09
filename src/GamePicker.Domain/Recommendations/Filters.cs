namespace GamePicker.Domain.Recommendations;

public enum GameMode
{
    Singleplayer,
    Multiplayer
}

public sealed record HardFilters
{
    public string? Platform { get; init; }
    public int? MaxDurationHours { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public GameMode? Mode { get; init; }
    public int? PlayerCount { get; init; }
    public string? Genre { get; init; }
    public int? ReleaseYearFrom { get; init; }
    public int? ReleaseYearTo { get; init; }
    public bool ExcludeOwnedAndPlayed { get; init; }

    public bool Matches(Game game, IReadOnlyDictionary<int, LibraryStatus> library) =>
        (Platform is null || game.Platforms.Contains(Platform))
        && (MaxDurationHours is null || game.DurationHours <= MaxDurationHours)
        && (MinPrice is null || game.Price >= MinPrice)
        && (MaxPrice is null || game.Price <= MaxPrice)
        && MatchesMode(game)
        && (Genre is null || game.Genre == Genre)
        && (ReleaseYearFrom is null || game.ReleaseYear >= ReleaseYearFrom)
        && (ReleaseYearTo is null || game.ReleaseYear <= ReleaseYearTo)
        && MatchesLibrary(game, library);

    private bool MatchesMode(Game game) => Mode switch
    {
        GameMode.Singleplayer => game.HasSingleplayer,
        GameMode.Multiplayer => game.MaxPlayers >= (PlayerCount ?? 2),
        _ => true
    };

    private bool MatchesLibrary(Game game, IReadOnlyDictionary<int, LibraryStatus> library)
    {
        if (!library.TryGetValue(game.Id, out var status))
            return true;

        return status != LibraryStatus.NotInterested && !ExcludeOwnedAndPlayed;
    }
}

public sealed record AuxiliaryFilters
{
    public string? Developer { get; init; }
    public string? Dimension { get; init; }

    public bool Matches(Game game) =>
        (Developer is null || game.Developer == Developer)
        && (Dimension is null || game.Dimension == Dimension);

    public IReadOnlyList<string> ActiveFilterNames()
    {
        var names = new List<string>();
        if (Developer is not null) names.Add("Kūrėjas");
        if (Dimension is not null) names.Add("2D / 3D");
        return names;
    }
}
