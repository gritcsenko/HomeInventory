namespace HomeInventory.Domain.Primitives;

public interface IValueObjectBuilder<TSelf, TObject, in TValue> : ISupplier<Optional<TObject>>, IResettable
    where TSelf : notnull, IValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    TSelf WithValue(TValue value);
}
