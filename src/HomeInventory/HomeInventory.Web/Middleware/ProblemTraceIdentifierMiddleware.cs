using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal class ProblemTraceIdentifierMiddleware(IScopeAccessor scopeAccessor) : IMiddleware
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(new TraceIdentifierContainer(context.TraceIdentifier)))
        {
            await next(context);
        }
    }
}
