using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Results;

namespace Fanex.DepositAPI.Domain.Services;

public interface ITicketService
{
    Task<ServiceResult<Ticket>> GetTicketInfomation(long refno);
}