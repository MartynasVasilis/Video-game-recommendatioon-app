using GamePicker.Domain.Recommendations;

namespace GamePicker.Application;

public sealed class RecommendationService(
    LibraryService library,
    IGameRepository games,
    RecommendationEngine engine)
{
    public async Task<RecommendationResult> RecommendAsync(int userId, RecommendationRequest request)
    {
        var entries = await library.GetAsync(userId);
        var catalog = await games.GetAllAsync();

        return engine.Recommend(request, catalog, entries.ToDictionary(e => e.GameId, e => e.Status));
    }
}
