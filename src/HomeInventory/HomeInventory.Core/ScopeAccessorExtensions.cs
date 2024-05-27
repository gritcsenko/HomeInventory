namespace HomeInventory.Core;

public static class ScopeAccessorExtensions
{
    public static IDisposable Set<TContext>(this IScopeAccessor accessor, TContext context)
        where TContext : class =>
        accessor.GetScope<TContext>().Set(context);

    public static IDisposable Reset<TContext>(this IScopeAccessor accessor)
        where TContext : class =>
        accessor.GetScope<TContext>().Reset();

    public static Optional<TContext> Get<TContext>(this IScopeAccessor accessor)
        where TContext : class =>
        accessor.GetScope<TContext>().Get();
}
