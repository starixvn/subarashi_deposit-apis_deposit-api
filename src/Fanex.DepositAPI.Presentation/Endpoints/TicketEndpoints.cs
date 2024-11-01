namespace Fanex.DepositAPI.Presentation.Endpoints;

using System.Net;
using Domain.Extensions;
using DTOs.Response;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Utils;

public class TicketEndpoints : IEndpoints
{
    public string GroupName => "Tickets";

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(this);

        group
            .MapGet(GetTicketInformation, "{refNo}");
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(GetTicketInformationResponse), (int)HttpStatusCode.OK)]
    internal static async Task<IResult> GetTicketInformation(
        long refNo,
        string? lang,
        IMediator mediator)
    {
        var query = ObjectMapper.MapGetTicketInfomationQuery(refNo);
        var result = await mediator.Send(query);

        return result.Match(
            _ => Results.Ok(new GetTicketInformationResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Result = result.Result
            }),
            (error, _) => Results.Ok(new GetTicketInformationResponse
            {
                ErrorCode = error.Code,
                ErrorMessage = error.Message
            })
        );
    }
}