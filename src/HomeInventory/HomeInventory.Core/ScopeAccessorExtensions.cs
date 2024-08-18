namespace HomeInventory.Core;

public static class ScopeAccessorExtensions
{
    public static TContext GetRequiredContext<TContext>(this IScopeAccessor scopeAccessor)
        where TContext : class =>
        scopeAccessor
            .GetScope<TContext>()
            .TryGet()
            .ThrowIfNone(() => new InvalidOperationException($"Required context of type {typeof(TContext).FullName} not found"));

    public static IDisposable Set<TContext>(this IScopeAccessor accessor, TContext context)
        where TContext : class =>
        accessor.GetScope<TContext>().Set(context);
}
