using HomeInventory.Core;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal class ProblemDetailsFactoryScopeInjectionMiddleware(IProblemDetailsFactory factory, IScopeAccessor scopeAccessor) : IMiddleware
{
    private readonly IProblemDetailsFactory _factory = factory;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(_factory))
        {
            await next(context);
        }
    }
}
