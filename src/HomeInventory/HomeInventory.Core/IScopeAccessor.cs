namespace HomeInventory.Core;

public interface IScopeAccessor
{
    IScope<TContext> GetScope<TContext>() where TContext : class;
}
