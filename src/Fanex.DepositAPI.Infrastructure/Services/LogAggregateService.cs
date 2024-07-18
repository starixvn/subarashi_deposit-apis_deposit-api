using System.Text;
using Fanex.DepositAPI.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Fanex.DepositAPI.Infrastructure.Services;

public class LogAggregateService : ILogAggregateService
{
    private readonly Logging.ILogger _logger;

    private StringBuilder Builder { get; set; }

    public LogAggregateService(Logging.ILogger logger)
    {
        Builder = new StringBuilder();
        _logger = logger;
    }

    public void AppendHeader(string message)
    => Builder.AppendLine($"{message.ToUpperInvariant()}");

    public void AppendLog(string message)
     => Builder.AppendLine($"{DateTime.Now}: {message}");

    public string GetAggregateMsg()
    {
        var logmsg = Builder.ToString();
        return logmsg;
    }

    public void SendLog(LogLevel logLevel)
    {
        if (Builder.Length == 0)
        {
            return;
        }
        switch (logLevel)
        {
            case LogLevel.Critical:
                _logger.Critical(Builder.ToString());
                break;

            case LogLevel.Error:
                _logger.Error(Builder.ToString());
                break;

            default: // Log Infomation
                _logger.Info(Builder.ToString());
                break;
        }

        Builder = new StringBuilder();
    }
}