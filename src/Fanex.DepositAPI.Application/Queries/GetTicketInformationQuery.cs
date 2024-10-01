namespace Fanex.DepositAPI.Application.Queries;

using Domain.Entities;
using Domain.Results;
using MediatR;

public class GetTicketInformationQuery : IRequest<ServiceResult<Ticket>>
{
    public long RefNo { get; set; }
}