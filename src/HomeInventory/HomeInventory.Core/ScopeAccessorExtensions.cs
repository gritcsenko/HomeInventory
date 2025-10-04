namespace HomeInventory.Core;

public static class ScopeAccessorExtensions
{
    public static TContext GetRequiredContext<TContext>(this IScopeAccessor scopeAccessor)
        where TContext : class =>
        scopeAccessor
            .GetScope<TContext>()
            .Get()
            .ThrowIfNone(static () => new InvalidOperationException($"Required context of type {typeof(TContext).FullName} not found"));
}
