using FluentResults;
using HomeInventory.Domain.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : IValueObject<TObject>
{
    protected static Result<TObject> TryCreate<TValue>(TValue value, Func<TValue, bool> isValidFunc, Func<TValue, TObject> createFunc)
        => TryCreate(() => isValidFunc(value), () => CreateValidationError(value), () => createFunc(value));

    protected static Result<TObject> TryCreate(Func<bool> isValidFunc, Func<IError> createErrorFunc, Func<TObject> createFunc)
        => Result.FailIf(!isValidFunc(), createErrorFunc()).Bind(() => Result.Ok(createFunc()));

    protected static IError CreateValidationError<TValue>(TValue value) => new ObjectValidationError<TValue>(value);
}
