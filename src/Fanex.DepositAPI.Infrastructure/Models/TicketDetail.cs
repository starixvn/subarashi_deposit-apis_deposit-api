namespace Fanex.DepositAPI.Infrastructure.Models;

using BetList.Core.Entities;

public class TicketDetail
{
    public Ticket MainTicket { get; set; }

    public List<ITicketData> TicketData { get; set; } = new();

    public int SpecialPlatform { get; set; }

    public int AgentId { get; set; }
}