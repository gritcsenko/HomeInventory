using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal abstract class BaseScopeInjectionMiddleware<TContext>(IScopeAccessor scopeAccessor) : IMiddleware
    where TContext : class
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(GetContext()))
        {
            await next(context);
        }
    }

    protected abstract TContext GetContext();
}
