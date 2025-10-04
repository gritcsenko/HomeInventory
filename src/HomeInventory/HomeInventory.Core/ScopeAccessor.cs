namespace HomeInventory.Core;

public sealed class ScopeAccessor(IScopeContainer container) : IScopeAccessor
{
    private readonly IScopeContainer _container = container;

    public IScope<TContext> GetScope<TContext>()
        where TContext : class =>
        _container.GetOrAdd<TContext>();
}
