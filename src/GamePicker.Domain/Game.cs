namespace GamePicker.Domain;

public sealed class Game
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string[] Platforms { get; init; }
    public required string Genre { get; init; }
    public string[] Tags { get; init; } = [];
    public string[] Moods { get; init; } = [];
    public int DurationHours { get; init; }
    public decimal Price { get; init; }
    public int ReleaseYear { get; init; }
    public bool HasSingleplayer { get; init; } = true;
    public int MaxPlayers { get; init; } = 1;
    public string Developer { get; init; } = "";
    public string Dimension { get; init; } = "2D";
}
