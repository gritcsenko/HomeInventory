namespace HomeInventory.Core;

public interface IScope
{
    IDisposable Reset();
}

public interface IScope<TContext> : IScope
    where TContext : class
{
    IDisposable Set(TContext context);

    Optional<TContext> Get();
}
