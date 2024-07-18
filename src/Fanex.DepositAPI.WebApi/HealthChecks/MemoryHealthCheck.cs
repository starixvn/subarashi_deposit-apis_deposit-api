using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fanex.DepositAPI.WebApi.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _memoryCache;

    private readonly long Threshold = 1024L;

    public MemoryHealthCheck(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false) / (1024L * 1024L);
        var usedMemory = new
        {
            AllocatedMBs = allocated,
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
        };

        var data = new Dictionary<string, object>
        {
            { "Memory Cache", _memoryCache.GetCurrentStatistics() },
            { "Memory Used", usedMemory },
        };

        var result = new HealthCheckResult(
            status: (allocated < Threshold) ? HealthStatus.Healthy : HealthStatus.Degraded,
            description: $"Reports degraded status if allocated bytes >= {Threshold} mbs.",
            exception: null,
            data: data);

        return Task.FromResult(result);
    }
}