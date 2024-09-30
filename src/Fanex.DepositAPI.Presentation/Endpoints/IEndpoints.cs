namespace Fanex.DepositAPI.Presentation.Endpoints;

using Microsoft.AspNetCore.Routing;

public interface IEndpoints
{
    string GroupName { get; }

    void Map(IEndpointRouteBuilder app);
}