namespace Fanex.DepositAPI.Domain.Entities;

public class Ticket
{
    public string RefNo { get; set; }

    public string SystemId { get; set; }

    public string LicTxID { get; set; }

    public string TransTime { get; set; }

    public string[] Choice { get; set; }

    public string Status { get; set; }
}