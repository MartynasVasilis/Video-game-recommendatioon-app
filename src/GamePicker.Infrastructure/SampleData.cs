using GamePicker.Domain;

namespace GamePicker.Infrastructure;

internal static class SampleData
{
    public static readonly User[] Users =
    [
        new() { Id = 1, Name = "Aistė" },
        new() { Id = 2, Name = "Mantas" },
        new() { Id = 3, Name = "Rokas" }
    ];

    public static readonly LibraryEntry[] LibraryEntries =
    [
        new() { UserId = 1, GameId = 4, Status = LibraryStatus.Played },
        new() { UserId = 2, GameId = 3, Status = LibraryStatus.NotInterested },
        new() { UserId = 3, GameId = 1, Status = LibraryStatus.Played },
        new() { UserId = 3, GameId = 7, Status = LibraryStatus.Owned }
    ];

    public static readonly Game[] Games =
    [
        new()
        {
            Id = 1, Title = "G1", Platforms = ["PC"], Genre = "Nuotykių", DurationHours = 10,
            Tags = ["Roguelike"], Moods = ["Atmosferiškas"],
            Price = 20, ReleaseYear = 2021, Developer = "Šiaurės studija", Dimension = "2D"
        },
        new()
        {
            Id = 2, Title = "G2", Platforms = ["PC"], Genre = "Veiksmo", DurationHours = 15,
            Tags = ["Roguelike"],
            Price = 25, ReleaseYear = 2022, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 3, Title = "G3", Platforms = ["PC"], Genre = "Nuotykių", DurationHours = 8,
            Moods = ["Atmosferiškas"],
            Price = 15, ReleaseYear = 2020, Developer = "Šiaurės studija", Dimension = "2D"
        },
        new()
        {
            Id = 4, Title = "G4", Platforms = ["PC"], Genre = "RPG", DurationHours = 40,
            Tags = ["Roguelike"],
            Price = 40, ReleaseYear = 2023, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 5, Title = "G5", Platforms = ["PS5"], Genre = "Veiksmo", DurationHours = 12,
            Tags = ["Roguelike"], Moods = ["Atmosferiškas"],
            Price = 50, ReleaseYear = 2024, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 6, Title = "G6", Platforms = ["PC"], Genre = "Galvosūkių", DurationHours = 12,
            Price = 10, ReleaseYear = 2019, Developer = "Šiaurės studija", Dimension = "2D"
        },
        new()
        {
            Id = 7, Title = "G7", Platforms = ["PC"], Genre = "Veiksmo", DurationHours = 9,
            Tags = ["Roguelike"],
            Price = 30, ReleaseYear = 2022, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 8, Title = "G8", Platforms = ["PC"], Genre = "Strategija", DurationHours = 60,
            Tags = ["Atviras pasaulis"], Moods = ["Įtemptas"], MaxPlayers = 4,
            Price = 35, ReleaseYear = 2024, Developer = "Baltijos žaidimai", Dimension = "3D"
        },
        new()
        {
            Id = 9, Title = "G9", Platforms = ["Switch"], Genre = "Simuliacija", DurationHours = 25,
            Moods = ["Jaukus"],
            Price = 20, ReleaseYear = 2023, Developer = "Jaukūs žaidimai", Dimension = "2D"
        },
        new()
        {
            Id = 10, Title = "G10", Platforms = ["PC", "PS5", "Xbox"], Genre = "Veiksmo", DurationHours = 6,
            Tags = ["Kooperatinis"], Moods = ["Įtemptas"], HasSingleplayer = false, MaxPlayers = 4,
            Price = 18, ReleaseYear = 2023, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 11, Title = "G11", Platforms = ["PC", "Switch"], Genre = "Simuliacija", DurationHours = 5,
            Tags = ["Kooperatinis"], Moods = ["Jaukus"], HasSingleplayer = false, MaxPlayers = 4,
            Price = 12, ReleaseYear = 2022, Developer = "Jaukūs žaidimai", Dimension = "2D"
        },
        new()
        {
            Id = 12, Title = "G12", Platforms = ["PC", "PS5", "Xbox"], Genre = "Lenktynių", DurationHours = 8,
            Moods = ["Įtemptas"], HasSingleplayer = false, MaxPlayers = 8,
            Price = 30, ReleaseYear = 2024, Developer = "Baltijos žaidimai", Dimension = "3D"
        },
        new()
        {
            Id = 13, Title = "G13", Platforms = ["PS5", "Xbox"], Genre = "Veiksmo", DurationHours = 18,
            Tags = ["Metroidvania"], Moods = ["Atmosferiškas"],
            Price = 35, ReleaseYear = 2021, Developer = "Šiaurės studija", Dimension = "2D"
        },
        new()
        {
            Id = 14, Title = "G14", Platforms = ["PC"], Genre = "RPG", DurationHours = 80,
            Tags = ["Atviras pasaulis"], Moods = ["Atmosferiškas"],
            Price = 60, ReleaseYear = 2024, Developer = "Šiaurės studija", Dimension = "3D"
        },
        new()
        {
            Id = 15, Title = "G15", Platforms = ["PS5", "Switch"], Genre = "Galvosūkių", DurationHours = 6,
            Moods = ["Jaukus"], MaxPlayers = 2,
            Price = 8, ReleaseYear = 2020, Developer = "Jaukūs žaidimai", Dimension = "2D"
        },
        new()
        {
            Id = 16, Title = "G16", Platforms = ["PC", "Xbox"], Genre = "Nuotykių", DurationHours = 25,
            Tags = ["Kooperatinis", "Atviras pasaulis"], Moods = ["Atmosferiškas"], MaxPlayers = 4,
            Price = 28, ReleaseYear = 2023, Developer = "Baltijos žaidimai", Dimension = "3D"
        },
        new()
        {
            Id = 17, Title = "G17", Platforms = ["PC"], Genre = "Veiksmo", DurationHours = 4,
            Moods = ["Įtemptas"], HasSingleplayer = false, MaxPlayers = 10,
            Price = 0, ReleaseYear = 2025, Developer = "Pixel Forge", Dimension = "3D"
        },
        new()
        {
            Id = 18, Title = "G18", Platforms = ["Xbox"], Genre = "Siaubo", DurationHours = 7,
            Moods = ["Įtemptas", "Atmosferiškas"],
            Price = 22, ReleaseYear = 2018, Developer = "Šiaurės studija", Dimension = "3D"
        },
        new()
        {
            Id = 19, Title = "G19", Platforms = ["PC", "Switch"], Genre = "Strategija", DurationHours = 30,
            Moods = ["Įtemptas"], MaxPlayers = 2,
            Price = 16, ReleaseYear = 2017, Developer = "Baltijos žaidimai", Dimension = "2D"
        },
        new()
        {
            Id = 20, Title = "G20", Platforms = ["Switch"], Genre = "Veiksmo", DurationHours = 14,
            Tags = ["Roguelike", "Metroidvania"], Moods = ["Įtemptas"],
            Price = 25, ReleaseYear = 2025, Developer = "Pixel Forge", Dimension = "2D"
        }
    ];
}
