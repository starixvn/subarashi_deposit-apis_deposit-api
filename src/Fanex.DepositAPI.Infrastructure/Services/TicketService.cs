using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Domain.Results;
using Fanex.DepositAPI.Domain.Services;

namespace Fanex.DepositAPI.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IBalanceUpRepository _balanceUpRepository;
    private readonly ICacheService _cacheService;

    public TicketService(
        ITicketRepository ticketRepository,
        ICacheService cacheService,
        IBalanceUpRepository balanceUpRepository)
    {
        _ticketRepository = ticketRepository;
        _cacheService = cacheService;
        _balanceUpRepository = balanceUpRepository;
    }

    public async Task<ServiceResult<Ticket>> GetTicketInfomation(long refno)
    {
        var balanceUp = await _cacheService.GetOrCreateFromCache(
                "Lrf_SelBalanceUpDay",
                () => _balanceUpRepository.GetBalanceUp(),
                TimeSpan.FromHours(1));

        var result = await _ticketRepository.GetTicketInfomation(refno, balanceUp);

        return result;
    }
}