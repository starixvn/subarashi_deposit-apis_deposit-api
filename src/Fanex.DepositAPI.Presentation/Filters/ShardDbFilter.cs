using Microsoft.AspNetCore.Http;

namespace Fanex.DepositAPI.Presentation.Filters;

public class ShardDbFilter<T> : IEndpointFilter where T : class
{
    //private readonly ISiteDbService _siteDbService;

    //public ShardDbFilter(ISiteDbService siteDbService)
    //{
    //    _siteDbService = siteDbService;
    //}

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        //(int custId, int agentId) = context.HttpContext.Request.Method switch
        //{
        //    "GET" => (0, 0),
        //    "PUT" => (0, 0),
        //    "POST" => (0, 0),
        //    _ => throw new NotImplementedException(),
        //};

        //var siteDB = await _siteDbService.GetSiteDB(custId, agentId);

        var siteDB = "10.18.200.60";

        context.HttpContext.Items["SiteDB"] = siteDB;

        return await next(context);
    }
}