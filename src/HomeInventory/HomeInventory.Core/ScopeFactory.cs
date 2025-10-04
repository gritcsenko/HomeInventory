namespace HomeInventory.Core;

public sealed class ScopeFactory : IScopeFactory
{
    public IScope<TContext> Create<TContext>()
        where TContext : class =>
        new Scope<TContext>();
}
