namespace HomeInventory.Domain.Primitives;

public static class PoolExtensions
{
    public static PoolHandle<T> PullScoped<T>(this IPool<T> pool)
        where T : class
    {
        var obj = pool.Pull();
        return new(pool, obj);
    }

    public readonly struct PoolHandle<T> : IDisposable
        where T : class
    {
        private readonly IPool<T> _pool;
        private readonly T _obj;

        public PoolHandle(IPool<T> pool, T obj)
        {
            _pool = pool;
            _obj = obj;
        }

        public T Object { get { return _obj; } }

        public void Dispose()
        {
            _pool.Push(_obj);
        }
    }
}
