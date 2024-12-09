using Microsoft.AspNetCore.Diagnostics;
using WebApi.Security.Exceptions;

namespace WebApi.ExceptionHandler;

public class AuthorizationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not AuthorizationException authorizationException) return false;

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            message = authorizationException.Message
        });

        return true;
    }
}