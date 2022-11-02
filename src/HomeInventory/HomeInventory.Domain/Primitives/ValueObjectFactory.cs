using FluentResults;
using HomeInventory.Domain.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectFactory<TObject>
    where TObject : IValueObject<TObject>
{
    protected Result<TObject> GetValidationError<TValue>(TValue value) => Result.Fail<TObject>(new ObjectValidationError<TValue>(value));
}
