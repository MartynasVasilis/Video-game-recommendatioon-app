using System.Text.Json.Serialization;
using GamePicker.Api;
using GamePicker.Application;
using GamePicker.Domain;
using GamePicker.Domain.Recommendations;
using GamePicker.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GamePickerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<RequestCurrentUser>();
builder.Services.AddScoped<ICurrentUser>(services => services.GetRequiredService<RequestCurrentUser>());
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
builder.Services.AddScoped<LibraryService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddSingleton<RecommendationEngine>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<GamePickerDbContext>().Database.Migrate();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Redirect("/api/users"));

app.MapGet("/api/users", (IUserRepository users) => users.GetAllAsync());

var api = app.MapGroup("/api").AddEndpointFilter<CurrentUserEndpointFilter>();

api.MapGet("/attributes", () => new { AttributeCatalog.Tags, AttributeCatalog.Moods });
api.MapGet("/filter-options", async (IGameRepository games) => FilterOptions.From(await games.GetAllAsync()));
api.MapGet("/games", (IGameRepository games) => games.GetAllAsync());

api.MapGet("/users/{userId:int}/library", (int userId, LibraryService library) => library.GetAsync(userId));

api.MapPut("/users/{userId:int}/library/{gameId:int}",
    async (int userId, int gameId, SetLibraryStatusRequest body, LibraryService library) =>
    {
        await library.SetStatusAsync(userId, gameId, body.Status);
        return Results.NoContent();
    });

api.MapDelete("/users/{userId:int}/library/{gameId:int}",
    async (int userId, int gameId, LibraryService library) =>
    {
        await library.RemoveAsync(userId, gameId);
        return Results.NoContent();
    });

api.MapPost("/users/{userId:int}/recommendations",
    (int userId, RecommendationRequest request, RecommendationService recommendations) =>
        recommendations.RecommendAsync(userId, request));

app.Run();

public record SetLibraryStatusRequest(LibraryStatus Status);

public partial class Program;
