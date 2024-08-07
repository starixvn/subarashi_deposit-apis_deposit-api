using Fanex.DepositAPI.Application.Queries;
namespace Fanex.DepositAPI.Presentation.Utils;

public static class ObjectMapper
{
    public static GetTicketInfomationQuery MapGetTicketInfomationQuery(long refno)
    {
        return new GetTicketInfomationQuery { Refno = refno };
    }
}