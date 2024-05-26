namespace HomeInventory.Domain.Primitives;

public readonly record struct PoolHandle<T>(IPool<T> Pool, T PooledObject) : IDisposable
    where T : class
{
    public void Dispose() => Pool.Push(PooledObject);
}
