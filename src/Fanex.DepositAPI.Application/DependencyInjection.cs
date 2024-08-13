using System.Reflection;
using Fanex.DepositAPI.Application.Logs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fanex.DepositAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.TryAddScoped<ILogAggregate, LogAggregate>();

        return services;
    }
}