namespace Fanex.DepositAPI.Presentation.DTOs.Response;

using Domain.Entities;
using Interfaces;

public class GetTicketInformationResponse : IResultResponse<Ticket>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public Ticket Result { get; set; }
}