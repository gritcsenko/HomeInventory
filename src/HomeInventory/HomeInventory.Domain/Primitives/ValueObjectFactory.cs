using HomeInventory.Domain.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : IValueObject<TObject>
{
    protected ValidationError GetValidationError<TValue>(TValue value) => new ObjectValidationError<TValue>(value);
}
