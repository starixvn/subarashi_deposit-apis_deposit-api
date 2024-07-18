using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Fanex.DepositAPI.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Fanex.DepositAPI.WebApi.HealthChecks;

public class ServerHealthCheck : IHealthCheck
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly AppSetting _appSetting;

    public ServerHealthCheck(
        IHttpContextAccessor httpContextAccessor,
        IOptions<AppSetting> appSetting)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSetting = appSetting.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        IDictionary<string, object> result = new Dictionary<string, object>
        {
            { "Env", _appSetting.Enviroment },
            { "Version", _appSetting.Version }
        };

        await AddServerIPInfo(result);
        AddExtraServerInfo(result);

        return HealthCheckResult.Healthy("Server Info", new ReadOnlyDictionary<string, object>(result));
    }

    private static async Task AddServerIPInfo(IDictionary<string, object> result)
    {
        var hostName = Dns.GetHostName();
        result.Add("Host Name", hostName);

        var serverIps = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);

        for (var i = 0; i < serverIps.Length; i++)
        {
            if (serverIps[i].AddressFamily == AddressFamily.InterNetwork)
            {
                result.Add($"IP{i}", serverIps[i].ToString());
            }
        }
    }

    private void AddExtraServerInfo(IDictionary<string, object> result)
    {
        var context = _httpContextAccessor.HttpContext;
        result.Add("Local Ip", context.Connection.LocalIpAddress.ToString());
        result.Add("Local Port", context.Connection.LocalPort);
        result.Add("Client Ip", context.Connection.RemoteIpAddress.ToString());
        result.Add("Server Time", DateTimeOffset.Now.LocalDateTime.ToString(CultureInfo.InvariantCulture));
        result.Add("Last Updated", File.GetCreationTime(typeof(ServerHealthCheck).Assembly.Location).ToString(CultureInfo.InvariantCulture));
        result.Add("Host", context.Request.Host);
        result.Add("Schema", context.Request.Scheme);
    }
}