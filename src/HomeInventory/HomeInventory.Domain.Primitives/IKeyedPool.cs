namespace HomeInventory.Domain.Primitives;

public interface IKeyedPool<TKey, T> : IPool
    where TKey : IEquatable<TKey>
    where T : class
{
    IPool<T> Get(TKey key);
}
