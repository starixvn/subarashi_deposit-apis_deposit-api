using Fanex.Data;

namespace Fanex.DepositAPI.Infrastructure.Repositories;

public interface IDbFactory
{
    IObjectDb GetObjectDb(string spName, int timeout = 120);
}