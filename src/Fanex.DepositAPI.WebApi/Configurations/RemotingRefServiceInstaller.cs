namespace Fanex.DepositAPI.WebApi.Configurations;

using BetList.Core;
using BetList.Core.RemotingRef;
using RemotingRef;

public class RemotingRefServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        RefDataTypeContainer
            .ConnectTo(new Uri(configuration["RemotingRefUrl"] ?? string.Empty))
            .FromApplication("DepositAPI");

        services.AddSingleton<IRemotingRefService, RemotingRefService>();

        BetListManager.RegisterRemotingRef(new CacheRef());
    }
}