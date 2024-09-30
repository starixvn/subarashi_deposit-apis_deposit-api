namespace Fanex.DepositAPI.Domain.Errors;

public static class TicketError
{
    public static readonly ServiceErrors NoInformation = new(404, "No information is available");

    public static readonly ServiceErrors SystemError = new(999, "Your request cannot be processed now, please try again later.");
}