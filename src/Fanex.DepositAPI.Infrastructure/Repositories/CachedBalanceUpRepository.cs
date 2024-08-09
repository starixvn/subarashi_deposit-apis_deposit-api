using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Infrastructure.Constants;
using Fanex.DepositAPI.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.DepositAPI.Infrastructure.Repositories;

public class CachedBalanceUpRepository : IBalanceUpRepository
{
    private readonly IBalanceUpRepository _decorated;
    private readonly IMemoryCache _cache;

    public CachedBalanceUpRepository(
        IBalanceUpRepository balanceUpRepository,
        IMemoryCache cache)
    {
        _decorated = balanceUpRepository;
        _cache = cache;
    }

    public async Task<BalanceUp> GetBalanceUp()
    {
        var result = await _cache.GetOrCreateFromCache(
            CacheKeys.BalanceUp,
            () => _decorated.GetBalanceUp(),
            TimeSpan.FromHours(1));

        return result;
    }
}