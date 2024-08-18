namespace HomeInventory.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface")]
public interface IScope
{
}

public interface IReadOnlyScope<TContext> : IScope
    where TContext : class
{
    Option<TContext> TryGet();
}

public interface IScope<TContext> : IReadOnlyScope<TContext>
    where TContext : class
{
    IDisposable Reset();

    IDisposable Set(TContext context);
}
