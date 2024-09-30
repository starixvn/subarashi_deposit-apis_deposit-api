namespace Fanex.DepositAPI.Application.Handlers;

using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using Domain.Results;
using Logging;
using MediatR;
using Queries;
using Utils;

public class GetTicketInformationHandler(
    ILogger logger,
    ITicketRepository ticketRepository,
    IBalanceUpRepository balanceUpRepository)
    : IRequestHandler<GetTicketInformationQuery, ServiceResult<Ticket>>
{
    public async Task<ServiceResult<Ticket>> Handle(GetTicketInformationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var balanceUp = await balanceUpRepository.GetBalanceUpAsync();

            var result = await ticketRepository.GetTicketInformationAsync(query.RefNo, balanceUp);

            return result;
        }
        catch (Exception exception)
        {
            await logger.ErrorAsync(ErrorMessageUtils.GenerateMessage(exception));

            return ServiceResult<Ticket>.Failure(TicketError.SystemError);
        }
    }
}