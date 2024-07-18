using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Fanex.DepositAPI.WebApi.ExceptionHandler;

internal sealed class BadHttpRequestExceptionHandler : IExceptionHandler
{
    private readonly Logging.ILogger _logger;

    public BadHttpRequestExceptionHandler(Logging.ILogger logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException)
        {
            return false;
        }

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        httpContext.Response.ContentType = "application/json";

        var result = new
        {
            code = 400,
            type = "INVALID_PARAMETER_TYPE",
            message = "The input parameters are of an incorrect type.",
        };

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}