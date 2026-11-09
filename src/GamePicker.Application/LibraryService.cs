using GamePicker.Domain;

namespace GamePicker.Application;

public sealed class LibraryService(
    IUserRepository users,
    IGameRepository games,
    ILibraryRepository library)
{
    public async Task<IReadOnlyList<LibraryEntry>> GetAsync(int userId)
    {
        await EnsureUserExistsAsync(userId);
        return await library.GetForUserAsync(userId);
    }

    public async Task SetStatusAsync(int userId, int gameId, LibraryStatus status)
    {
        await EnsureUserExistsAsync(userId);
        if (!await games.ExistsAsync(gameId))
            throw new NotFoundException($"Žaidimas {gameId} nerastas.");

        await library.SetStatusAsync(userId, gameId, status);
    }

    public async Task RemoveAsync(int userId, int gameId)
    {
        await EnsureUserExistsAsync(userId);
        await library.RemoveAsync(userId, gameId);
    }

    private async Task EnsureUserExistsAsync(int userId)
    {
        if (!await users.ExistsAsync(userId))
            throw new NotFoundException($"Naudotojas {userId} nerastas.");
    }
}
