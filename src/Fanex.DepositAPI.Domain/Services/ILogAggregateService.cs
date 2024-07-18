using Microsoft.Extensions.Logging;

namespace Fanex.DepositAPI.Domain.Services;

public interface ILogAggregateService
{
    void AppendHeader(string message);

    void AppendLog(string message);

    string GetAggregateMsg();

    void SendLog(LogLevel logLevel);
}