namespace Fanex.DepositAPI.Infrastructure;

using BetList.Core.Builder;
using BetList.Core.Entities;
using BetList.Core.TicketHelper;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repositories;
using Repositories.Decorate;
using Settings;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSetting>(configuration.GetSection(AppSetting.SettingName));
        services.Configure<SentrySetting>(configuration.GetSection(SentrySetting.SettingName));

        services.TryAddScoped<ITicketHelper, TicketHelper>();
        services.TryAddScoped<IBetListExcelBuilder, BetListExcelBuilder>();
        services.TryAddScoped<IBetListHTMLBuilder, BetListHTMLBuilder>();

        services.AddMemoryCache(options => { options.TrackStatistics = true; });

        // repositories

        services.TryAddScoped<ITicketRepository, TicketRepository>();
        services.TryAddSingleton<IDbFactory, DbFactory>();

        services.AddDecorateRepository();

        return services;
    }

    private static void AddDecorateRepository(this IServiceCollection services)
    {
        services.TryAddScoped<IBalanceUpRepository, BalanceUpRepository>();
        services.TryDecorate<IBalanceUpRepository, CachedBalanceUpRepository>();
    }
}