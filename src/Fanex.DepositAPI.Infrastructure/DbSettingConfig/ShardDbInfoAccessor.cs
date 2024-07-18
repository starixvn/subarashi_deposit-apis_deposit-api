using Microsoft.AspNetCore.Http;

namespace Fanex.DepositAPI.Infrastructure.DBSettingConfig;

public class ShardDbInfoAccessor : IShardDbInfoAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly string _prefixSharding = "shard_";

    public ShardDbInfoAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
    }

    public string GetShardSPKey(string key) => $"SiteDB_{key.Substring(_prefixSharding.Length)}";

    public bool TryUseShardDB(string key, out string dbServer)
    {
        if (IsSharding(key) && !string.IsNullOrEmpty(SiteDBServer))
        {
            dbServer = SiteDBServer;
            return true;
        }
        dbServer = null;
        return false;
    }

    private bool IsSharding(string key) => key.StartsWith(_prefixSharding, StringComparison.OrdinalIgnoreCase);

    private string SiteDBServer => _contextAccessor.HttpContext.Items["SiteDB"].ToString();
}