namespace GamePicker.Domain;

public enum LibraryStatus
{
    Owned,
    Played,
    NotInterested
}

public sealed class LibraryEntry
{
    public int UserId { get; init; }
    public int GameId { get; init; }
    public LibraryStatus Status { get; set; }
}
