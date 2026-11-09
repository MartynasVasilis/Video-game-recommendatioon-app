namespace GamePicker.Domain.Recommendations;

public sealed record RecommendationRequest
{
    public const int MinWeight = 0;
    public const int MaxWeight = 5;

    public HardFilters Hard { get; init; } = new();
    public AuxiliaryFilters Auxiliary { get; init; } = new();
    public IReadOnlyDictionary<string, int> Weights { get; init; } = new Dictionary<string, int>();
    public int Count { get; init; } = 3;
    public int? MaxPerGenre { get; init; }

    public void Validate(FilterOptions options)
    {
        if (Hard is null || Auxiliary is null || Weights is null)
            throw new InvalidRecommendationRequestException(["Laukai „hard“, „auxiliary“ ir „weights“ negali būti null."]);

        var errors = new List<string>();

        void CheckAllowed(string? value, IReadOnlyList<string> allowed, string field)
        {
            if (value is not null && !allowed.Contains(value))
                errors.Add($"Laukas „{field}“ neturi reikšmės „{value}“.");
        }

        CheckAllowed(Hard.Platform, options.Platforms, "Platforma");
        CheckAllowed(Hard.Genre, options.Genres, "Žanras");
        CheckAllowed(Auxiliary.Developer, options.Developers, "Kūrėjas");
        CheckAllowed(Auxiliary.Dimension, options.Dimensions, "2D / 3D");

        if (Count < 1)
            errors.Add("Rekomendacijų skaičius N turi būti ne mažesnis nei 1.");
        if (MaxPerGenre < 1)
            errors.Add("Įvairovės riba K turi būti ne mažesnė nei 1.");
        if (Hard.MaxDurationHours < 1)
            errors.Add("Maksimali trukmė turi būti ne mažesnė nei 1 valanda.");
        if (Hard.MinPrice < 0 || Hard.MaxPrice < 0)
            errors.Add("Kaina negali būti neigiama.");
        if (Hard.MinPrice > Hard.MaxPrice)
            errors.Add("Minimali kaina negali viršyti maksimalios.");
        if (Hard.ReleaseYearFrom > Hard.ReleaseYearTo)
            errors.Add("Leidimo metų intervalo pradžia negali būti vėlesnė už pabaigą.");
        if (Hard.PlayerCount is not null && Hard.Mode != GameMode.Multiplayer)
            errors.Add("Žaidėjų skaičių galima nurodyti tik „multiplayer“ režimui.");
        if (Hard.PlayerCount < 2)
            errors.Add("„Multiplayer“ režimui reikia bent 2 žaidėjų.");

        foreach (var (name, weight) in Weights)
        {
            if (!AttributeCatalog.Contains(name))
                errors.Add($"Nežinoma žymė arba nuotaika: {name}.");
            if (weight is < MinWeight or > MaxWeight)
                errors.Add($"Svertas „{name}“ turi būti nuo {MinWeight} iki {MaxWeight}.");
        }

        if (errors.Count > 0)
            throw new InvalidRecommendationRequestException(errors);
    }
}

public sealed class InvalidRecommendationRequestException(IReadOnlyList<string> errors)
    : Exception(string.Join(" ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors;
}
