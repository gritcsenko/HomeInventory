namespace HomeInventory.Core;

public interface IScopeFactory
{
    IScope<TContext> Create<TContext>() where TContext : class;
}
