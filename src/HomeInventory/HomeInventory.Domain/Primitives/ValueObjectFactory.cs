using FluentResults;
using HomeInventory.Domain.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : notnull, ValueObject<TObject>
{
    protected Result<TObject> GetValidationError<TValue>(TValue value) => Result.Fail<TObject>(GetValidationErrorCore(value));

    protected virtual Error GetValidationErrorCore<TValue>(TValue value) => new ObjectValidationError<TValue>(value);
}
