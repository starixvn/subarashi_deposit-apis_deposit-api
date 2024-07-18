using System.Reflection;
using Fanex.DepositAPI.Application;
using Fanex.DepositAPI.Infrastructure;
using Fanex.DepositAPI.Presentation;
using Fanex.DepositAPI.WebApi.Configurations;
using Fanex.DepositAPI.WebApi.ExceptionHandler;
using Fanex.DepositAPI.WebApi.HealthChecks;

namespace Fanex.DepositAPI.WebApi.Startup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddPresentation();

        services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

        services.Configure<RouteHandlerOptions>(options =>
        {
            options.ThrowOnBadRequest = true;
        });

        services
            .AddHealthChecks()
            .AddCheck<ServerHealthCheck>("server_whoami")
            .AddCheck<MemoryHealthCheck>("memory_whoami");

        services.AddHttpContextAccessor();
        services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.InstallServices(configuration, typeof(IServiceInstaller).Assembly);

        return services;
    }

    private static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var serviceInstallers = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && t.IsClass)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (IServiceInstaller serviceInstaller in serviceInstallers)
        {
            serviceInstaller.Install(services, configuration);
        }

        return services;
    }
}