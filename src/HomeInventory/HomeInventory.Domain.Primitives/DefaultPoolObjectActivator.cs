namespace HomeInventory.Domain.Primitives;

internal sealed class DefaultPoolObjectActivator<T> : FunctionPoolObjectActivator<T>
    where T : class
{
    public DefaultPoolObjectActivator()
        : base(Activator.CreateInstance<T>, _ => { })
    {
    }

    public static IPoolObjectActivator<T> Instance { get; } = new DefaultPoolObjectActivator<T>();
}
