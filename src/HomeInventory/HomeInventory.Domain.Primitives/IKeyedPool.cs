namespace HomeInventory.Domain.Primitives;

public interface IKeyedPool<TKey, T> : IPool
    where TKey : IEquatable<TKey>
    where T : class
{
    IReadOnlyCollection<TKey> Keys { get; }

    IPool<T> GetPool(TKey key);
}
