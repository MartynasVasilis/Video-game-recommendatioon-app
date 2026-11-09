using GamePicker.Application;
using GamePicker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GamePicker.Api;

public sealed class RequestCurrentUser : ICurrentUser
{
    public int? UserId { get; set; }
}
public sealed class CurrentUserEndpointFilter(RequestCurrentUser currentUser, GamePickerDbContext db) : IEndpointFilter
{
    public const string HeaderName = "X-User";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocation, EndpointFilterDelegate next)
    {
        string? header = invocation.HttpContext.Request.Headers[HeaderName];

        if (!int.TryParse(header, out var userId) || !await db.Users.IgnoreQueryFilters().AnyAsync(u => u.Id == userId))
            return Results.Problem(
                $"Nurodykite galiojantį naudotoją antraštėje {HeaderName}.",
                statusCode: StatusCodes.Status400BadRequest);

        currentUser.UserId = userId;
        return await next(invocation);
    }
}
