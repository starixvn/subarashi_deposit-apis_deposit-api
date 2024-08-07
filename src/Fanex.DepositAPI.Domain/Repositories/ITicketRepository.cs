using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Results;

namespace Fanex.DepositAPI.Domain.Repositories;

public interface ITicketRepository
{
    Task<ServiceResult<Ticket>> GetTicketInfomation(long refno, BalanceUp balanceUp);
}