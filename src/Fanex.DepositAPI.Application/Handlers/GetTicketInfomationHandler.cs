using Fanex.DepositAPI.Application.Queries;
using Fanex.DepositAPI.Application.Utils;
using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Errors;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Domain.Results;
using Fanex.Logging;
using MediatR;

namespace Fanex.DepositAPI.Application.Handlers;

public class GetTicketInfomationHandler : IRequestHandler<GetTicketInfomationQuery, ServiceResult<Ticket>>
{
    private readonly ILogger _logger;
    private readonly IBalanceUpRepository _balanceUpRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetTicketInfomationHandler(
        ILogger logger,
        ITicketRepository ticketRepository,
        IBalanceUpRepository balanceUpRepository)
    {
        _logger = logger;
        _ticketRepository = ticketRepository;
        _balanceUpRepository = balanceUpRepository;
    }

    public async Task<ServiceResult<Ticket>> Handle(GetTicketInfomationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var balanceUp = await _balanceUpRepository.GetBalanceUp();

            var result = await _ticketRepository.GetTicketInfomation(query.Refno, balanceUp);

            return result;
        }
        catch (Exception exception)
        {
            _logger.Error(ErrorMessageUtils.GenerateMessage(exception));

            return ServiceResult<Ticket>.Failure(TicketError.SystemError);
        }
    }
}