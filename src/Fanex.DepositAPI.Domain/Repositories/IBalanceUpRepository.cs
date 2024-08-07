using Fanex.DepositAPI.Domain.Entities;

namespace Fanex.DepositAPI.Domain.Repositories;

public interface IBalanceUpRepository
{
    Task<BalanceUp> GetBalanceUp();
}