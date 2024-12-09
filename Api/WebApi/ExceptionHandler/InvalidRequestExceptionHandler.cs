using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.ExceptionHandler;

public class InvalidRequestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException) return false;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = ((ValidationException)exception).Errors.First().ErrorMessage;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = details,
        }, cancellationToken: cancellationToken);
        return true;
    }
}