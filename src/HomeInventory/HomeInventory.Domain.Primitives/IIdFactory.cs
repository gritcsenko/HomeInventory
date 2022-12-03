namespace HomeInventory.Domain.Primitives;

public interface IIdFactory<out TId>
    where TId : IIdentifierObject<TId>
{
    TId CreateNew();
}

public interface IIdFactory<TId, TValue> : IIdFactory<TId>, IValueObjectFactory<TId, TValue>
    where TId : IIdentifierObject<TId>
{
}
