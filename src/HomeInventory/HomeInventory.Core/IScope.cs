namespace HomeInventory.Core;

public interface IScope<TContext>
    where TContext : class
{
    IDisposable Set(TContext context);
    IDisposable Reset();
    Optional<TContext> TryGet();
}
