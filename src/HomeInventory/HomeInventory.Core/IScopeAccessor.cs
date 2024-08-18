namespace HomeInventory.Core;

public interface IScopeAccessor
{
    IScope<TContext> GetScope<TContext>() where TContext : class;

    IReadOnlyScope<TContext> GetReadOnlyScope<TContext>() where TContext : class;
}
