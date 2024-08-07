using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Results;
using MediatR;

namespace Fanex.DepositAPI.Application.Queries
{
    public class GetTicketInfomationQuery : IRequest<ServiceResult<Ticket>>
    {
        public long Refno { get; set; }
    }
}