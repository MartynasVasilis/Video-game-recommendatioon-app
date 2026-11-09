using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace GamePicker.Tests;

public class RecommendationApiTests(ApiFixture api) : IClassFixture<ApiFixture>
{
    private const string Url = "/api/users/1/recommendations";

    [Fact]
    public async Task Main_scenario_returns_ranked_games_from_the_database()
    {
        var response = await api.ClientFor("1").PostAsJsonAsync(Url, new
        {
            hard = new { platform = "PC", maxDurationHours = 12, mode = "Singleplayer" },
            weights = new Dictionary<string, int> { ["Roguelike"] = 3, ["Atmosferiškas"] = 2 },
            count = 3,
            maxPerGenre = 2
        });

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var games = result.GetProperty("games").EnumerateArray()
            .Select(game => $"{game.GetProperty("title").GetString()} {game.GetProperty("score").GetInt32()}");
        Assert.Equal(["G1 5", "G7 3", "G3 2"], games);
    }

    [Theory]
    [InlineData(20, 2, new[] { "G1", "G7" })]
    [InlineData(6, 3, new string[0])]
    public async Task Sample_catalog_keeps_the_design_document_scenarios(int maxHours, int count, string[] expected)
    {
        var titles = await TitlesAsync(new
        {
            hard = new { platform = "PC", maxDurationHours = maxHours, mode = "Singleplayer" },
            weights = new Dictionary<string, int> { ["Roguelike"] = 3, ["Atmosferiškas"] = 2 },
            count,
            maxPerGenre = 2
        });

        Assert.Equal(expected, titles);
    }

    [Fact]
    public async Task Multiplayer_returns_only_games_for_that_many_players()
    {
        var titles = await TitlesAsync(new
        {
            hard = new { platform = "PC", mode = "Multiplayer", playerCount = 8 },
            count = 5
        });

        Assert.Equal(["G17", "G12"], titles);
    }

    [Fact]
    public async Task Invalid_request_returns_400_with_reason()
    {
        var response = await api.ClientFor("1").PostAsJsonAsync(Url, new { count = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Rekomendacijų skaičius N turi būti ne mažesnis nei 1.", problem.GetProperty("errors")[0].GetString());
    }

    [Fact]
    public async Task Malformed_json_returns_400()
    {
        var body = new StringContent("{ not json", Encoding.UTF8, "application/json");

        var response = await api.ClientFor("1").PostAsync(Url, body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("""{ "hard": null }""")]
    [InlineData("""{ "auxiliary": null }""")]
    [InlineData("""{ "weights": null }""")]
    public async Task Null_section_returns_400(string json)
    {
        var body = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await api.ClientFor("1").PostAsync(Url, body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<List<string?>> TitlesAsync(object request)
    {
        var response = await api.ClientFor("1").PostAsJsonAsync(Url, request);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("games").EnumerateArray().Select(game => game.GetProperty("title").GetString()).ToList();
    }
}
