using Fanex.DepositAPI.Application.Queries;
using Fanex.DepositAPI.Application.Utils;
using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Errors;
using Fanex.DepositAPI.Domain.Results;
using Fanex.DepositAPI.Domain.Services;
using Fanex.Logging;
using MediatR;

namespace Fanex.DepositAPI.Application.Handlers;

public class GetTicketInfomationHandler : IRequestHandler<GetTicketInfomationQuery, ServiceResult<Ticket>>
{
    private readonly ILogger _logger;
    private readonly ITicketService _ticketService;

    public GetTicketInfomationHandler(
        ILogger logger,
        ITicketService ticketService)
    {
        _logger = logger;
        _ticketService = ticketService;
    }

    public async Task<ServiceResult<Ticket>> Handle(GetTicketInfomationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ticketService.GetTicketInfomation(query.Refno);

            return result;
        }
        catch (Exception exception)
        {
            _logger.Error(ErrorMessageUtils.GenerateMessage(exception));

            return ServiceResult<Ticket>.Failure(TicketError.SystemError);
        }
    }
}