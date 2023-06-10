namespace HomeInventory.Domain.Primitives;

internal class FunctionPoolObjectActivator<T> : IPoolObjectActivator<T>
    where T : class
{
    private readonly Func<T> _createFunc;
    private readonly Action<T> _onObjectReturned;

    public FunctionPoolObjectActivator(Func<T> createFunc, Action<T> onObjectReturned)
    {
        _createFunc = createFunc;
        _onObjectReturned = onObjectReturned;
    }

    public T Pull() => _createFunc();

    public void Push(T obj)
    {
        _onObjectReturned(obj);
    }
}
