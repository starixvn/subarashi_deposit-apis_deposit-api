using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Fanex.DepositAPI.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fanex.DepositAPI.Presentation.Extensions;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGroup(this IEndpointRouteBuilder app, IEndpoints group)
    {
        var groupName = group.GroupName;

        var routeHandler = app
            .MapGroup($"/api/{groupName.ToLower()}")
            //.WithGroupName(groupName) // use when have multiple version swagger
            .WithTags(groupName)
            .WithOpenApi();

        return routeHandler;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointType = typeof(IEndpoints);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetExportedTypes()
            .Where(t => t.IsClass && endpointType.IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            if (Activator.CreateInstance(type) is IEndpoints instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }

    public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapGet(pattern.ToLower(), handler)
            .WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapPost(pattern.ToLower(), handler)
            .WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapPut(pattern.ToLower(), handler)
            .WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapDelete(pattern.ToLower(), handler)
            .WithName(handler.Method.Name);
    }
}