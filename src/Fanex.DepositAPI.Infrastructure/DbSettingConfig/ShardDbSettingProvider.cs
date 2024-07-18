using Fanex.Data;
using Microsoft.Data.SqlClient;

namespace Fanex.DepositAPI.Infrastructure.DBSettingConfig;

public class ShardDbSettingProvider : XmlDbSettingProvider
{
    private readonly IShardDbInfoAccessor _dbInfoAccessor;

    public ShardDbSettingProvider(IShardDbInfoAccessor dbInfoAccessor, string resourcePath) : base(resourcePath)
    {
        _dbInfoAccessor = dbInfoAccessor;
    }

    public override IDbSetting GetSetting(string key)
    {
        if (_dbInfoAccessor.TryUseShardDB(key, out string dbServer))
        {
            var shardSPKey = _dbInfoAccessor.GetShardSPKey(key);
            var originalShardSetting = base.GetSetting(shardSPKey);

            var connectionStr = new SqlConnectionStringBuilder(originalShardSetting.ConnectionString)
            {
                DataSource = dbServer
            };

            var dbSetting = new DbSetting(shardSPKey, originalShardSetting.DbName, connectionStr.ToString(), originalShardSetting.SpName,
                originalShardSetting.CommandTimeout, originalShardSetting.ColumnMaps, originalShardSetting.FirmParameters);
            return dbSetting;
        }
        return base.GetSetting(key);
    }
}