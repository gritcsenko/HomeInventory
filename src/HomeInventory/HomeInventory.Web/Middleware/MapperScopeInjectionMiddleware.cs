using AutoMapper;
using HomeInventory.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
