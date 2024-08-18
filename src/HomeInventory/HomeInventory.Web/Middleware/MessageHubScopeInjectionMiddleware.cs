using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Web.Middleware;

internal class MessageHubScopeInjectionMiddleware(IMessageHub hub, IScopeAccessor scopeAccessor) : BaseScopeInjectionMiddleware<IMessageHub>(scopeAccessor)
{
    private readonly IMessageHub _hub = hub;

    protected override IMessageHub GetContext() => _hub;
}