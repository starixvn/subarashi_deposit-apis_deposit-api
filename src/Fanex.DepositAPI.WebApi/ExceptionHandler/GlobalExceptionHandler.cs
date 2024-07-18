using System.Net;
using Fanex.DepositAPI.Application.Utils;
using Microsoft.AspNetCore.Diagnostics;

namespace Fanex.DepositAPI.WebApi.ExceptionHandler;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly Logging.ILogger _logger;

    public GlobalExceptionHandler(Logging.ILogger logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var errorMsg = ErrorMessageUtils.GenerateMessage(exception, httpContext);
        _logger.Error(errorMsg, exception);

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var result = new
        {
            code = 500,
            type = "",
            message = "An internal server error occurred while a request was processed."
        };

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}