using Fanex.BetList.Core.RemotingRef;
using Fanex.BetList.Core;
using Fanex.RemotingRef;

namespace Fanex.DepositAPI.WebApi.Configurations;

public class RemotingRefServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        RefDataTypeContainer
        .ConnectTo(new Uri(configuration["RemotingRefUrl"]))
        .FromApplication("DepositAPI");

        services.AddSingleton<IRemotingRefService, RemotingRefService>();

        BetListManager.RegisterRemotingRef(new CacheRef());
    }
}