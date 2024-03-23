namespace HomeInventory.Domain.Primitives;

public class KeyedPool<TKey, T>(Func<TKey, IPool<T>> poolFactory) : IKeyedPool<TKey, T>
    where TKey : notnull, IEquatable<TKey>
    where T : class
{
    private readonly Dictionary<TKey, IPool<T>> _pools = [];
    private readonly Func<TKey, IPool<T>> _poolFactory = poolFactory;

    public int Count => _pools.Values.Sum(pool => pool.Count);

    public IReadOnlyCollection<TKey> Keys => _pools.Keys;

    public void Clear()
    {
        foreach (var pool in _pools.Values)
            pool.Clear();
    }

    public IPool<T> GetPool(TKey key) => _pools.GetOrAdd(key, _poolFactory);
}
