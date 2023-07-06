using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal class AreaModule : ApiModule
{
    public AreaModule()
        : base("/api/areas"/*, PermissionType.AccessAreas*/)
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapGet("", GetAllAsync)
            .AllowAnonymous()
            //.RequireDynamicAuthorization(PermissionType.ReadArea)
            ;
    }

    public static async Task<IResult> GetAllAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();
        ////var apiVersion = context.GetRequestedApiVersion();
        var result = await context.GetSender().Send(new AllAreasQuery(), cancellationToken);
        return context.MatchToOk(result, mapper.Map<AreaResponse[]>);
    }
}
