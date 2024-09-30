namespace Fanex.DepositAPI.WebApi.Configurations;

using Data;
using Data.SqlServer;

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

        DbSettingProviderManager
            .StartNewSession()
            .Use(connectionStrings)
            .WithSqlServer()
            .UseDefaultDbSettingProvider("~/FanexSpName.xml")
            .Run();
    }
}