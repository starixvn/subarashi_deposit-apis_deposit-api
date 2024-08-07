using Fanex.BetList.Core.Entities;

namespace Fanex.DepositAPI.Infrastructure.Models;

public class TicketDetail
{
    public Ticket MainTicket { get; set; }

    public List<ITicketData> TicketData { get; set; } = new List<ITicketData>();

    public int SpecialPlatform { get; set; }
}