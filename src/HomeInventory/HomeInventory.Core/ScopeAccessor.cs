using System.Collections.Concurrent;

namespace HomeInventory.Core;

public sealed class ScopeAccessor : IScopeAccessor
{
    private readonly ConcurrentDictionary<Type, object> _scopes = new();

    public IScope<TContext> GetScope<TContext>()
        where TContext : class =>
        (Scope<TContext>)_scopes.GetOrAdd(typeof(TContext), _ => new Scope<TContext>());
}
