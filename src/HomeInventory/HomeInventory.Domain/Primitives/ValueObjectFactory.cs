using FluentResults;
using HomeInventory.Domain.Errors;
using OneOf;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : IValueObject<TObject>
{
    protected static OneOf<TObject, IError> TryCreate<TValue>(TValue value, Func<TValue, bool> isValidFunc, Func<TValue, TObject> createFunc)
        => TryCreate(() => isValidFunc(value), () => CreateValidationError(value), () => createFunc(value));

    protected static OneOf<TObject, IError> TryCreate(Func<bool> isValidFunc, Func<Error> createErrorFunc, Func<TObject> createFunc)
    {
        if (isValidFunc())
        {
            return createFunc();
        }

        return createErrorFunc();
    }

    protected static Error CreateValidationError<TValue>(TValue value) => new ObjectValidationError<TValue>(value);

    protected Result<TObject> GetValidationError<TValue>(TValue value) => Result.Fail<TObject>(GetValidationErrorCore(value));

    protected virtual Error GetValidationErrorCore<TValue>(TValue value) => new ObjectValidationError<TValue>(value);
}
