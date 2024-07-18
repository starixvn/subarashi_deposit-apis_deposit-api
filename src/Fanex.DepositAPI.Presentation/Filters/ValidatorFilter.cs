using System.ComponentModel.DataAnnotations;
using Fanex.DepositAPI.Presentation.Utils;
using Microsoft.AspNetCore.Http;

namespace Fanex.DepositAPI.Presentation.Filters;

public class ValidatorFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var entity = context.Arguments.OfType<T>().FirstOrDefault(a => a?.GetType() == typeof(T));

        if (RequestValidator.IsValidRequest(entity, out List<ValidationResult> validationResults))
        {
            return await next(context);
        }

        return Results.BadRequest(new
        {
            code = 400,
            type = "INVALID_PARAMETER_VALUE",
            message = $"Invalid value or required parameter list: {RequestValidator.GetInvalidParamList(validationResults)}",
        });
    }
}