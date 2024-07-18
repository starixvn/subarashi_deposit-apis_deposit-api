using Fanex.DepositAPI.Domain.Services;
using Fanex.DepositAPI.Infrastructure.Repositories;
using Fanex.DepositAPI.Infrastructure.Services;
using Fanex.DepositAPI.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fanex.DepositAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSetting>(configuration.GetSection(AppSetting.SettingName));
        services.Configure<SentrySetting>(configuration.GetSection(SentrySetting.SettingName));

        // services
        services.TryAddScoped<ILogAggregateService, LogAggregateService>();
        services.TryAddTransient<ICacheService, CacheService>();
        services.AddMemoryCache(options =>
        {
            options.TrackStatistics = true;
        });

        // repositories
        services.TryAddSingleton<IDbFactory, DbFactory>();
        return services;
    }
}