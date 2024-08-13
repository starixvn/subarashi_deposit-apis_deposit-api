using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Infrastructure.Utils;

namespace Fanex.DepositAPI.Infrastructure.Repositories;

public class BalanceUpRepository : IBalanceUpRepository
{
    private readonly IDbFactory _dbFactory;

    public BalanceUpRepository(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<BalanceUp> GetBalanceUp()
    {
        const string SpKey = "Lrf_SelBalanceUpDay";

        using (var db = _dbFactory.GetObjectDb(SpKey))
        {
            var balanceUp = new BalanceUp();
            using (var reader = await db.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    balanceUp.BalanceUpDay = Converter.ToDateTime(reader["balanceupday"]);
                    balanceUp.MoveBetsDay = Converter.ToDateTime(reader["movebetsday"]);
                }

                return balanceUp;
            }
        }
    }
}