using System.Net;
using Fanex.DepositAPI.Domain.Extensions;
using Fanex.DepositAPI.Presentation.DTOs.Response;
using Fanex.DepositAPI.Presentation.Extensions;
using Fanex.DepositAPI.Presentation.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fanex.DepositAPI.Presentation.Endpoints;

public class TicketEndpoints : IEndpoints
{
    public string GroupName => "Tickets";

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(this);

        group
            .MapGet(GetTicketInfomation, "{refno}");
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(GetTicketInfomationResponse), (int)HttpStatusCode.OK)]
    private static async Task<IResult> GetTicketInfomation(
        long refno,
        string? lang,
        IMediator mediator)
    {
        var query = ObjectMapper.MapGetTicketInfomationQuery(refno);
        var result = await mediator.Send(query);

        return result.Match(
            onSuccess: () => Results.Ok(new GetTicketInfomationResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Result = result.Result,
            }),
            onFailure: (error) => Results.Ok(new GetTicketInfomationResponse
            {
                ErrorCode = error.Code,
                ErrorMessage = error.Message,
            })
        );
    }
}