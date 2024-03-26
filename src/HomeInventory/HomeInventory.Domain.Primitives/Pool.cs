namespace HomeInventory.Domain.Primitives;

internal sealed class Pool<T> : FunctionPoolObjectActivator<T>, IPool<T>
    where T : class
{
    private readonly Queue<T> _pool;
    private readonly IPoolObjectActivator<T> _activator;

    public Pool(IPoolObjectActivator<T>? activator = null)
        : this(new(), activator ?? DefaultPoolObjectActivator<T>.Instance)
    {
    }

    private Pool(Queue<T> pool, IPoolObjectActivator<T> activator)
        : base(() => pool.TryDequeue(out var value) ? value : activator.Pull(), obj => { pool.Enqueue(obj); activator.Push(obj); })
    {
        _pool = pool;
        _activator = activator;
    }

    public int Count => _pool.Count;

    public void Fill(int count)
    {
        _pool.EnsureCapacity(_pool.Count + count);
        foreach (var item in _activator.Pull(count))
        {
            _pool.Enqueue(item);
        }
    }

    public void Clear() => _pool.Clear();
}
