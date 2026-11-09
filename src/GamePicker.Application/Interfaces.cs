using GamePicker.Domain;

namespace GamePicker.Application;

public interface IGameRepository
{
    Task<IReadOnlyList<Game>> GetAllAsync();
    Task<bool> ExistsAsync(int gameId);
}

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();

    Task<bool> ExistsAsync(int userId);
}

public interface ILibraryRepository
{
    Task<IReadOnlyList<LibraryEntry>> GetForUserAsync(int userId);
    Task SetStatusAsync(int userId, int gameId, LibraryStatus status);
    Task RemoveAsync(int userId, int gameId);
}

public interface ICurrentUser
{
    int? UserId { get; }
}

public sealed class NotFoundException(string message) : Exception(message);
