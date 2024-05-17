namespace HomeInventory.Core;

public sealed class ScopeAccessor : IScopeAccessor
{
    public IScope<TContext> GetScope<TContext>()
        where TContext : class =>
        new Scope<TContext>();
}
