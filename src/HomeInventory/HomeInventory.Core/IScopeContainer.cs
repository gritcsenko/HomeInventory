namespace HomeInventory.Core;

public interface IScopeContainer
{
    IScope<TContext> GetOrAdd<TContext>() where TContext : class;
}
