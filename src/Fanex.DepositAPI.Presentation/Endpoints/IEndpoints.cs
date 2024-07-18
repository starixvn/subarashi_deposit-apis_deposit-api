using Microsoft.AspNetCore.Routing;

namespace Fanex.DepositAPI.Presentation.Endpoints;

public interface IEndpoints
{
    string GroupName { get; }

    void Map(IEndpointRouteBuilder app);
}