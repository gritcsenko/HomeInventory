using System.Collections.Concurrent;

namespace HomeInventory.Core;

public sealed class ScopeContainer(IScopeFactory factory) : IScopeContainer
{
    private readonly ConcurrentDictionary<Type, IScope> _scopes = new();

    private readonly IScopeFactory _factory = factory;

    public IScope<TContext> GetOrAdd<TContext>()
        where TContext : class =>
        (IScope<TContext>)_scopes.GetOrAdd(typeof(TContext), _ => _factory.Create<TContext>());
}
