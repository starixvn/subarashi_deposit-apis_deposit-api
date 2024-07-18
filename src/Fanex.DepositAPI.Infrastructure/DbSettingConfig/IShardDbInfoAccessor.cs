namespace Fanex.DepositAPI.Infrastructure.DBSettingConfig;

public interface IShardDbInfoAccessor
{
    bool TryUseShardDB(string key, out string dbServer);

    string GetShardSPKey(string key);
}