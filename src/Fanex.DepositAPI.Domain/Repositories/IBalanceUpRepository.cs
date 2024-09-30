namespace Fanex.DepositAPI.Domain.Repositories;

using Entities;

public interface IBalanceUpRepository
{
    Task<BalanceUp> GetBalanceUpAsync();
}