namespace Fanex.DepositAPI.Domain.Entities;

public class Ticket
{
    public int SportType { get; set; }

    public int SiteId { get; set; }

    public int AgentId { get; set; }

    public string RefNo { get; set; }

    public string SystemId { get; set; }

    public string LicTxID { get; set; }

    public string TransTime { get; set; }

    public string[] Choice { get; set; }

    public string Status { get; set; }
}