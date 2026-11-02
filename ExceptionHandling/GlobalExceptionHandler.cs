using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Nafs.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Detail = "An unexpected error occurred."
        };

        if (exception is ArgumentException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Bad request";
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode =
            problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}