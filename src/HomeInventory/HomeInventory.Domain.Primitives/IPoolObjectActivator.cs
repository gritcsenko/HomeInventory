namespace HomeInventory.Domain.Primitives;

public interface IPoolObjectActivator<T>
    where T : class
{
    T Pull();

    void Push(T obj);
}
