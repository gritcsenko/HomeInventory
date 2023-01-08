namespace HomeInventory.Domain.Primitives;

public interface IPool
{
    int Count { get; }

    void Clear();
}

public interface IPool<T> : IPool, IPoolObjectActivator<T>
    where T : class
{
    void Fill(int count);
}
