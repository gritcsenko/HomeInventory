using AutoMapper;
using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Contracts;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class AreaModule(IMapper mapper, ISender sender, IProblemDetailsFactory factory) : ApiModule("/api/areas"/*, PermissionType.AccessAreas*/)
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IProblemDetailsFactory _factory = factory;

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapGet("", GetAllAsync)
            .AllowAnonymous()
            //.RequireDynamicAuthorization(PermissionType.ReadArea)
            ;
    }

    public async Task<IResult> GetAllAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        ////var apiVersion = context.GetRequestedApiVersion();
        var result = await _sender.Send(new AllAreasQuery(), cancellationToken);
        return _factory.MatchToOk(result, _mapper.Map<AreaResponse[]>);
    }
}
