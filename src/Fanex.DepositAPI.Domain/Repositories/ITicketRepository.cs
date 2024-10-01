namespace Fanex.DepositAPI.Domain.Repositories;

using Entities;
using Results;

public interface ITicketRepository
{
    Task<ServiceResult<Ticket>> GetTicketInformationAsync(long refNo, BalanceUp balanceUp);
}