namespace Fanex.DepositAPI.Infrastructure.Repositories;

using Constants;
using Domain.Entities;
using Domain.Repositories;
using Utils;

public class BalanceUpRepository(IDbFactory dbFactory) : IBalanceUpRepository
{
    public async Task<BalanceUp> GetBalanceUpAsync()
    {
        const string SpKey = "Lrf_SelBalanceUpDay";

        using var db = dbFactory.GetObjectDb(SpKey);
        using var reader = await db.ExecuteReaderAsync();

        if (reader.Read())
        {
            return new BalanceUp
            {
                BalanceUpDay = Converter.ToDateTime(reader[DbParam.BalanceUpDay]),
                MoveBetsDay = Converter.ToDateTime(reader[DbParam.MoveBetsDay])
            };
        }

        return default;
    }
}