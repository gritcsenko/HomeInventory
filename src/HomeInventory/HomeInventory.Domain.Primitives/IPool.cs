namespace HomeInventory.Domain.Primitives;

public interface IPool
{
    int Count { get; }

    void Clear();

    void Fill(int count);
}

public interface IPool<T> : IPool, IPoolObjectActivator<T>
    where T : class
{
}
