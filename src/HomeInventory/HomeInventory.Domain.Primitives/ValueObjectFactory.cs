using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : IValueObject<TObject>
{
    protected static OneOf<TObject, IError> TryCreate<TValue>(TValue value, Func<TValue, bool> isValidFunc, Func<TValue, TObject> createFunc)
    {
        if (isValidFunc(value))
        {
            return createFunc(value);
        }

        return new ObjectValidationError<TValue>(value);
    }
}
