using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GamePicker.Application;
using GamePicker.Domain;
using GamePicker.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace GamePicker.Tests;

public sealed class ApiFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(host =>
            host.UseSetting("ConnectionStrings:Default", _database.GetConnectionString()));
        _factory.CreateClient();
    }

    public HttpClient ClientFor(string? user)
    {
        var client = _factory.CreateClient();
        if (user is not null)
            client.DefaultRequestHeaders.Add("X-User", user);
        return client;
    }

    public GamePickerDbContext DatabaseFor(int userId)
    {
        var options = new DbContextOptionsBuilder<GamePickerDbContext>().UseNpgsql(_database.GetConnectionString()).Options;
        return new GamePickerDbContext(options, new FixedUser(userId));
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _database.DisposeAsync();
    }

    private sealed record FixedUser(int? UserId) : ICurrentUser;
}

public class ClientIsolationTests(ApiFixture api) : IClassFixture<ApiFixture>
{
    private const string Aiste = "1";
    private const string Rokas = "3";

    [Fact]
    public async Task Each_client_sees_only_its_own_library()
    {
        var aiste = await GameIdsAsync(Aiste);
        var rokas = await GameIdsAsync(Rokas);

        Assert.Contains(4, aiste);
        Assert.DoesNotContain(1, aiste);
        Assert.Equal([1, 7], rokas);
    }

    [Fact]
    public async Task Client_cannot_read_library_of_another_client()
    {
        var response = await api.ClientFor(Aiste).GetAsync($"/api/users/{Rokas}/library");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Client_cannot_get_recommendations_for_another_client()
    {
        var response = await api.ClientFor(Aiste).PostAsJsonAsync($"/api/users/{Rokas}/recommendations", new { count = 3 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Client_cannot_change_library_of_another_client()
    {
        var aiste = api.ClientFor(Aiste);
        var libraryBefore = await api.ClientFor(Rokas).GetStringAsync($"/api/users/{Rokas}/library");

        var update = await aiste.PutAsJsonAsync($"/api/users/{Rokas}/library/1", new { status = "NotInterested" });
        var create = await aiste.PutAsJsonAsync($"/api/users/{Rokas}/library/2", new { status = "Owned" });
        var delete = await aiste.DeleteAsync($"/api/users/{Rokas}/library/1");

        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, create.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
        Assert.Equal(libraryBefore, await api.ClientFor(Rokas).GetStringAsync($"/api/users/{Rokas}/library"));
    }

    [Fact]
    public async Task Database_query_without_any_condition_returns_only_the_current_clients_entries()
    {
        await using var db = api.DatabaseFor(userId: 3);

        var owners = await db.LibraryEntries.Select(entry => entry.UserId).Distinct().ToListAsync();

        Assert.Equal([3], owners);
    }

    [Fact]
    public async Task Database_refuses_to_save_an_entry_of_another_client()
    {
        await using var db = api.DatabaseFor(userId: 1);
        db.LibraryEntries.Add(new LibraryEntry { UserId = 3, GameId = 2, Status = LibraryStatus.Owned });

        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("abc")]
    [InlineData("999")]
    public async Task Request_without_a_valid_client_is_rejected(string? user)
    {
        var response = await api.ClientFor(user).GetAsync($"/api/users/{Rokas}/library");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Recommendations_use_the_library_of_the_requesting_client()
    {
        var request = new
        {
            hard = new { platform = "PC", excludeOwnedAndPlayed = true },
            weights = new Dictionary<string, int> { ["Roguelike"] = 3 },
            count = 1
        };

        var response = await api.ClientFor(Rokas).PostAsJsonAsync($"/api/users/{Rokas}/recommendations", request);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("G2", result.GetProperty("games")[0].GetProperty("title").GetString());
    }

    private async Task<List<int>> GameIdsAsync(string user)
    {
        var library = await api.ClientFor(user).GetFromJsonAsync<JsonElement>($"/api/users/{user}/library");
        return library.EnumerateArray().Select(entry => entry.GetProperty("gameId").GetInt32()).Order().ToList();
    }
}
