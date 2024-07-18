using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.DepositAPI.Infrastructure.DBSettingConfig;

namespace Fanex.DepositAPI.WebApi.Configurations;

public class FanexDataServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var listConnectionName = configuration.GetSection("ConnectionStrings").GetChildren().Select(_ => _.Key);
        var connectionStrings = listConnectionName
            .Select(connectionName => new KeyValuePair<string, ConnectionConfiguration>(
                connectionName,
                new ConnectionConfiguration(connectionName, configuration.GetConnectionString(connectionName))))
            .ToDictionary();

        //var httpContext = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();

        //DbSettingProviderManager
        //    .StartNewSession()
        //    .Use(connectionStrings)
        //    .WithSqlServer()
        //    .UseDefaultDbSettingProvider("~/FanexSpName.xml")
        //    //.Use(new ShardDbSettingProvider(new ShardDbInfoAccessor(httpContext), "~/FanexSpName.xml"))
        //    .Run();
    }
}