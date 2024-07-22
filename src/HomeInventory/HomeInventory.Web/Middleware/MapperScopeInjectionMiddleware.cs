using AutoMapper;
using HomeInventory.Core;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal class MapperScopeInjectionMiddleware(IMapper mapper, IScopeAccessor scopeAccessor) : IMiddleware
{
    private readonly IMapper _mapper = mapper;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(_mapper))
        {
            await next(context);
        }
    }
}
