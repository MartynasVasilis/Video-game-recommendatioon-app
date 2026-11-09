using GamePicker.Application;
using GamePicker.Domain;
using Microsoft.EntityFrameworkCore;

namespace GamePicker.Infrastructure;

public sealed class GameRepository(GamePickerDbContext db) : IGameRepository
{
    public async Task<IReadOnlyList<Game>> GetAllAsync() =>
        await db.Games.AsNoTracking().OrderBy(g => g.Id).ToListAsync();

    public Task<bool> ExistsAsync(int gameId) => db.Games.AnyAsync(g => g.Id == gameId);
}

public sealed class UserRepository(GamePickerDbContext db) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync() =>
        await db.Users.IgnoreQueryFilters().AsNoTracking().OrderBy(u => u.Id).ToListAsync();

    public Task<bool> ExistsAsync(int userId) => db.Users.AnyAsync(u => u.Id == userId);
}

public sealed class LibraryRepository(GamePickerDbContext db) : ILibraryRepository
{
    public async Task<IReadOnlyList<LibraryEntry>> GetForUserAsync(int userId) =>
        await db.LibraryEntries.AsNoTracking().Where(e => e.UserId == userId).ToListAsync();

    public async Task SetStatusAsync(int userId, int gameId, LibraryStatus status)
    {
        var entry = await FindAsync(userId, gameId);
        if (entry is null)
            db.LibraryEntries.Add(new LibraryEntry { UserId = userId, GameId = gameId, Status = status });
        else
            entry.Status = status;

        await db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int userId, int gameId)
    {
        var entry = await FindAsync(userId, gameId);
        if (entry is null)
            return;

        db.LibraryEntries.Remove(entry);
        await db.SaveChangesAsync();
    }

    private Task<LibraryEntry?> FindAsync(int userId, int gameId) =>
        db.LibraryEntries.SingleOrDefaultAsync(e => e.UserId == userId && e.GameId == gameId);
}
