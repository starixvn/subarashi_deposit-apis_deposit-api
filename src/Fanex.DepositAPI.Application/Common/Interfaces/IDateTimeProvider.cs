namespace Fanex.DepositAPI.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }

    public DateTime MinValue { get; }
}