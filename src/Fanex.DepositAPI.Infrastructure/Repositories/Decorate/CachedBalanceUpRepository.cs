namespace Fanex.DepositAPI.Infrastructure.Repositories.Decorate;

using Constants;
using Domain.Entities;
using Domain.Repositories;
using Extensions;
using Microsoft.Extensions.Caching.Memory;

public class CachedBalanceUpRepository(
    IBalanceUpRepository balanceUpRepository,
    IMemoryCache cache) : IBalanceUpRepository
{
    public async Task<BalanceUp> GetBalanceUpAsync()
    {
        var result = await cache.GetOrCreateFromCache(
            CacheKeys.BalanceUp,
            balanceUpRepository.GetBalanceUpAsync,
            TimeSpan.FromHours(1));

        return result;
    }
}