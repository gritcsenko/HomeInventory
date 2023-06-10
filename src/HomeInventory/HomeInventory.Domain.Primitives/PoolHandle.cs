namespace HomeInventory.Domain.Primitives;

public readonly struct PoolHandle<T> : IDisposable, IEquatable<PoolHandle<T>>
    where T : class
{
    private readonly IPool<T> _pool;
    private readonly T _obj;

    public PoolHandle(IPool<T> pool, T obj)
    {
        _pool = pool;
        _obj = obj;
    }

    public T PooledObject { get { return _obj; } }

    public void Dispose()
    {
        _pool.Push(_obj);
    }

    public override bool Equals(object? obj)
    {
        return obj is PoolHandle<T> handle && Equals(handle);
    }

    public bool Equals(PoolHandle<T> other)
    {
        return EqualityComparer<IPool<T>>.Default.Equals(_pool, other._pool) &&
               EqualityComparer<T>.Default.Equals(_obj, other._obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_pool, _obj);
    }

    public static bool operator ==(PoolHandle<T> left, PoolHandle<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PoolHandle<T> left, PoolHandle<T> right)
    {
        return !(left == right);
    }
}
