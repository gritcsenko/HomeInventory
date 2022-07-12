using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public interface IValueObjectFactory<TObject, TValue>
    where TObject : notnull, IValueObject<TObject, TValue>
{
    ErrorOr<TObject> Create(TValue value);
}
