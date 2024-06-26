using HomeInventory.Core;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal class MessageHubScopeInjectionMiddleware(IMessageHub hub, IScopeAccessor scopeAccessor) : IMiddleware
{
    private readonly IMessageHub _hub = hub;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(_hub))
        {
            await next(context);
        }
    }
}