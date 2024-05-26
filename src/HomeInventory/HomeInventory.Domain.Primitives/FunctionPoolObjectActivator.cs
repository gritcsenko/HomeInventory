namespace HomeInventory.Domain.Primitives;

internal class FunctionPoolObjectActivator<T>(Func<T> createFunc, Action<T> onObjectReturned) : IPoolObjectActivator<T>
    where T : class
{
    private readonly Func<T> _createFunc = createFunc;
    private readonly Action<T> _onObjectReturned = onObjectReturned;

    public T Pull() => _createFunc();

    public void Push(T obj)
    {
        _onObjectReturned(obj);
    }
}
