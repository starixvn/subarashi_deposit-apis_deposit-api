using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Presentation.DTOs.Interfaces;

namespace Fanex.DepositAPI.Presentation.DTOs.Response;

public class GetTicketInfomationResponse : IResultResponse<Ticket>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public Ticket Result { get; set; }
}