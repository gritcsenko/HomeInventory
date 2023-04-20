namespace HomeInventory.Domain.Primitives;

internal sealed class Pool<T> : IPool<T>
    where T : class
{
    private readonly Queue<T> _pool = new();
    private readonly IPoolObjectActivator<T> _activator;

    public Pool(IPoolObjectActivator<T> activator) => _activator = activator;

    public int Count => _pool.Count;

    public T Pull() => _pool.TryDequeue(out var value) ? value : _activator.Pull();

    public void Fill(int count)
    {
        for (var i = 0; i < count; i++)
        {
            _pool.Enqueue(_activator.Pull());
        }
    }

    public void Clear() => _pool.Clear();

    public void Push(T obj)
    {
        _pool.Enqueue(obj);
        _activator.Push(obj);
    }
}
