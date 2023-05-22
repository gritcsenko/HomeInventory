using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IValueObjectBuilder<TSelf, TObject, TValue> : ISupplier<TObject>, IResettable
    where TSelf : notnull, IValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    public TSelf WithValue<TSupplier>(in TSupplier value)
        where TSupplier : ISupplier<TValue>;
}
