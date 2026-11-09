namespace GamePicker.Domain;

public static class AttributeCatalog
{
    public static readonly IReadOnlyList<string> Tags = ["Roguelike", "Metroidvania", "Kooperatinis", "Atviras pasaulis"];
    public static readonly IReadOnlyList<string> Moods = ["Atmosferiškas", "Jaukus", "Įtemptas"];

    public static bool Contains(string name) => Tags.Contains(name) || Moods.Contains(name);
}
