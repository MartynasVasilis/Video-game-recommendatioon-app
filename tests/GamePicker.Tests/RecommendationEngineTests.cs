using GamePicker.Domain;
using GamePicker.Domain.Recommendations;

namespace GamePicker.Tests;

public class RecommendationEngineTests
{
    private static readonly Game[] Catalog =
    [
        NewGame(1, "PC", 10, "Nuotykių", tags: ["Roguelike"], moods: ["Atmosferiškas"], developer: "Studija A"),
        NewGame(2, "PC", 15, "Veiksmo", tags: ["Roguelike"]),
        NewGame(3, "PC", 8, "Nuotykių", moods: ["Atmosferiškas"]),
        NewGame(4, "PC", 40, "RPG", tags: ["Roguelike"]),
        NewGame(5, "PS5", 12, "Veiksmo", tags: ["Roguelike"], moods: ["Atmosferiškas"]),
        NewGame(6, "PC", 12, "Galvosūkių"),
        NewGame(7, "PC", 9, "Veiksmo", tags: ["Roguelike"])
    ];

    private static readonly Dictionary<string, int> Weights = new() { ["Roguelike"] = 3, ["Atmosferiškas"] = 2 };
    private static readonly Dictionary<int, LibraryStatus> EmptyLibrary = [];

    private readonly RecommendationEngine _engine = new();

    [Fact]
    public void Normal_case_returns_best_games_with_score_breakdown()
    {
        var result = _engine.Recommend(Request(maxHours: 12, count: 3, maxPerGenre: 2), Catalog, EmptyLibrary);

        Assert.Equal(["G1 5", "G7 3", "G3 2"], Summary(result));
        Assert.Equal([new ScoreLine("Roguelike", 3), new ScoreLine("Atmosferiškas", 2)], result.Games[0].Breakdown);
        Assert.Empty(result.RemovedFilters);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Equal_score_prefers_shorter_game()
    {
        var result = _engine.Recommend(Request(maxHours: 20, count: 2, maxPerGenre: 2), Catalog, EmptyLibrary);

        Assert.Equal(["G1 5", "G7 3"], Summary(result));
    }

    [Fact]
    public void Equal_score_prefers_more_matching_attributes_before_duration()
    {
        Game[] catalog =
        [
            NewGame(1, "PC", 1, "Veiksmo", tags: ["Roguelike"]),
            NewGame(2, "PC", 50, "Veiksmo", tags: ["Metroidvania"], moods: ["Atmosferiškas"])
        ];
        var weights = new Dictionary<string, int> { ["Roguelike"] = 3, ["Metroidvania"] = 1, ["Atmosferiškas"] = 2 };

        var result = _engine.Recommend(Request(weights: weights, count: 2), catalog, EmptyLibrary);

        Assert.Equal(["G2 3", "G1 3"], Summary(result));
    }

    [Fact]
    public void Equal_score_matches_and_duration_fall_back_to_title()
    {
        Game[] catalog = [NewGame(2, "PC", 10, "Veiksmo"), NewGame(1, "PC", 10, "Veiksmo")];

        var result = _engine.Recommend(Request(count: 2), catalog, EmptyLibrary);

        Assert.Equal(["G1 0", "G2 0"], Summary(result));
    }

    [Fact]
    public void Same_input_in_any_catalog_order_gives_identical_result()
    {
        var expected = Summary(_engine.Recommend(Request(count: 7), Catalog, EmptyLibrary));

        var actual = Summary(_engine.Recommend(Request(count: 7), Catalog.Reverse(), EmptyLibrary));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void No_candidates_returns_empty_list_with_message()
    {
        var result = _engine.Recommend(Request(maxHours: 6, count: 3, maxPerGenre: 2), Catalog, EmptyLibrary);

        Assert.Empty(result.Games);
        Assert.Equal(RecommendationEngine.NothingFoundMessage, result.Message);
    }

    [Fact]
    public void Diversity_limit_replaces_surplus_game_with_next_by_score()
    {
        var result = _engine.Recommend(Request(maxHours: 20, count: 3, maxPerGenre: 1), Catalog, EmptyLibrary);

        Assert.Equal(["G1 5", "G7 3", "G6 0"], Summary(result));
    }

    [Fact]
    public void Zero_weight_does_not_count_as_a_match()
    {
        var weights = new Dictionary<string, int> { ["Roguelike"] = 0, ["Atmosferiškas"] = 2 };

        var result = _engine.Recommend(Request(weights: weights, count: 1), Catalog, EmptyLibrary);

        Assert.Equal([new ScoreLine("Atmosferiškas", 2)], result.Games[0].Breakdown);
    }

    [Fact]
    public void Auxiliary_filter_is_kept_when_it_leaves_candidates()
    {
        var request = Request(count: 3) with { Auxiliary = new AuxiliaryFilters { Developer = "Studija A" } };

        var result = _engine.Recommend(request, Catalog, EmptyLibrary);

        Assert.Equal(["G1 5"], Summary(result));
        Assert.Empty(result.RemovedFilters);
    }

    [Fact]
    public void Auxiliary_filters_are_removed_and_reported_when_nothing_matches()
    {
        var request = Request(maxHours: 9, count: 1) with
        {
            Auxiliary = new AuxiliaryFilters { Developer = "Studija A", Dimension = "2D" }
        };

        var result = _engine.Recommend(request, Catalog, EmptyLibrary);

        Assert.Equal(["G7 3"], Summary(result));
        Assert.Equal(["Kūrėjas", "2D / 3D"], result.RemovedFilters);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Hard_filters_are_never_relaxed()
    {
        var request = Request(maxHours: 6, count: 3) with { Auxiliary = new AuxiliaryFilters { Developer = "Studija A" } };

        var result = _engine.Recommend(request, Catalog, EmptyLibrary);

        Assert.Empty(result.Games);
        Assert.Equal(["Kūrėjas"], result.RemovedFilters);
        Assert.Equal(RecommendationEngine.NothingFoundMessage, result.Message);
    }

    [Theory]
    [InlineData(false, "G1 5")]
    [InlineData(true, "G7 3")]
    public void Owned_and_played_games_are_excluded_only_on_request(bool exclude, string expectedFirst)
    {
        var library = new Dictionary<int, LibraryStatus> { [1] = LibraryStatus.Played };
        var request = Request(maxHours: 12, count: 1) with
        {
            Hard = new HardFilters { Platform = "PC", MaxDurationHours = 12, ExcludeOwnedAndPlayed = exclude }
        };

        var result = _engine.Recommend(request, Catalog, library);

        Assert.Equal([expectedFirst], Summary(result));
    }

    [Fact]
    public void Not_interested_game_is_always_excluded()
    {
        var library = new Dictionary<int, LibraryStatus> { [1] = LibraryStatus.NotInterested };

        var result = _engine.Recommend(Request(maxHours: 12, count: 1), Catalog, library);

        Assert.Equal(["G7 3"], Summary(result));
    }

    [Fact]
    public void Multiplayer_filter_requires_enough_player_slots()
    {
        Game[] catalog =
        [
            NewGame(1, "PC", 5, "Veiksmo", maxPlayers: 2),
            NewGame(2, "PC", 5, "Veiksmo", maxPlayers: 4),
            NewGame(3, "PC", 5, "Veiksmo")
        ];
        var request = Request(count: 3) with
        {
            Hard = new HardFilters { Mode = GameMode.Multiplayer, PlayerCount = 3 }
        };

        var result = _engine.Recommend(request, catalog, EmptyLibrary);

        Assert.Equal(["G2 0"], Summary(result));
    }

    [Theory]
    [InlineData(0, null, 3, "Rekomendacijų skaičius N turi būti ne mažesnis nei 1.")]
    [InlineData(3, 0, 3, "Įvairovės riba K turi būti ne mažesnė nei 1.")]
    [InlineData(3, null, 6, "Svertas „Roguelike“ turi būti nuo 0 iki 5.")]
    public void Invalid_request_is_rejected_with_a_reason(int count, int? maxPerGenre, int weight, string expectedError)
    {
        var request = Request(count: count, maxPerGenre: maxPerGenre,
            weights: new Dictionary<string, int> { ["Roguelike"] = weight });

        var exception = Assert.Throws<InvalidRecommendationRequestException>(
            () => _engine.Recommend(request, Catalog, EmptyLibrary));

        Assert.Equal([expectedError], exception.Errors);
    }

    [Fact]
    public void Weight_for_attribute_outside_the_fixed_list_is_rejected()
    {
        var request = Request(weights: new Dictionary<string, int> { ["Neegzistuojanti"] = 1 });

        var exception = Assert.Throws<InvalidRecommendationRequestException>(
            () => _engine.Recommend(request, Catalog, EmptyLibrary));

        Assert.Equal(["Nežinoma žymė arba nuotaika: Neegzistuojanti."], exception.Errors);
    }

    [Fact]
    public void Filter_value_missing_from_the_catalog_is_rejected()
    {
        var request = Request() with
        {
            Hard = new HardFilters { Platform = "pc", Genre = "Veiksmas" },
            Auxiliary = new AuxiliaryFilters { Developer = "Studja A", Dimension = "4D" }
        };

        var exception = Assert.Throws<InvalidRecommendationRequestException>(
            () => _engine.Recommend(request, Catalog, EmptyLibrary));

        Assert.Equal(
            [
                "Laukas „Platforma“ neturi reikšmės „pc“.",
                "Laukas „Žanras“ neturi reikšmės „Veiksmas“.",
                "Laukas „Kūrėjas“ neturi reikšmės „Studja A“.",
                "Laukas „2D / 3D“ neturi reikšmės „4D“."
            ],
            exception.Errors);
    }

    [Fact]
    public void Negative_price_and_zero_duration_are_rejected()
    {
        var request = Request() with { Hard = new HardFilters { MaxDurationHours = 0, MinPrice = -1 } };

        var exception = Assert.Throws<InvalidRecommendationRequestException>(
            () => _engine.Recommend(request, Catalog, EmptyLibrary));

        Assert.Equal(
            ["Maksimali trukmė turi būti ne mažesnė nei 1 valanda.", "Kaina negali būti neigiama."],
            exception.Errors);
    }

    [Fact]
    public void Filter_options_are_distinct_values_from_the_catalog()
    {
        var options = FilterOptions.From(Catalog);

        Assert.Equal(["PC", "PS5"], options.Platforms);
        Assert.Equal(["Studija A", "Studija B"], options.Developers);
    }

    private static RecommendationRequest Request(
        int? maxHours = null,
        int count = 3,
        int? maxPerGenre = null,
        IReadOnlyDictionary<string, int>? weights = null) => new()
    {
        Hard = new HardFilters { Platform = "PC", MaxDurationHours = maxHours, Mode = GameMode.Singleplayer },
        Weights = weights ?? Weights,
        Count = count,
        MaxPerGenre = maxPerGenre
    };

    private static Game NewGame(
        int id,
        string platform,
        int hours,
        string genre,
        string[]? tags = null,
        string[]? moods = null,
        string developer = "Studija B",
        int maxPlayers = 1) => new()
    {
        Id = id,
        Title = $"G{id}",
        Platforms = [platform],
        DurationHours = hours,
        Genre = genre,
        Tags = tags ?? [],
        Moods = moods ?? [],
        Developer = developer,
        MaxPlayers = maxPlayers
    };

    private static List<string> Summary(RecommendationResult result) =>
        result.Games.Select(game => $"{game.Title} {game.Score}").ToList();
}
