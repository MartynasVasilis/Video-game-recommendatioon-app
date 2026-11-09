using GamePicker.Application;
using GamePicker.Domain.Recommendations;
using Microsoft.AspNetCore.Diagnostics;

namespace GamePicker.Api;

public sealed class ErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception exception, CancellationToken cancellationToken)
    {
        IResult? result = exception switch
        {
            InvalidRecommendationRequestException invalid => Results.Problem(
                invalid.Message,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?> { ["errors"] = invalid.Errors }),
            NotFoundException notFound => Results.Problem(notFound.Message, statusCode: StatusCodes.Status404NotFound),
            BadHttpRequestException badRequest => Results.Problem(
                "Neteisingas užklausos formatas.",
                statusCode: badRequest.StatusCode),
            _ => null
        };

        if (result is null)
            return false;

        await result.ExecuteAsync(http);
        return true;
    }
}
