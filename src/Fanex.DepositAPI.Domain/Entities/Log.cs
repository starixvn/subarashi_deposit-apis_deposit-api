namespace Fanex.DepositAPI.Domain.Entities;

public class Log
{
    public int UserId { get; set; }

    public int CustID { get; set; }

    public int ActionID { get; set; }

    public string TrackingValue { get; set; }

    public string TrackingValue2 { get; set; }
}