using Microsoft.Extensions.Logging;

namespace Fanex.DepositAPI.Application.Logs;

public interface ILogAggregate
{
    void AppendHeader(string message);

    void AppendLog(string message);

    string GetAggregateMsg();

    void SendLog(LogLevel logLevel);
}