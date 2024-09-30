namespace Fanex.DepositAPI.Presentation.Utils;

using Application.Queries;

public static class ObjectMapper
{
    public static GetTicketInformationQuery MapGetTicketInfomationQuery(long refNo)
    {
        return new GetTicketInformationQuery { RefNo = refNo };
    }
}