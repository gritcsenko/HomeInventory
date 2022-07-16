using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public abstract class ValueObjectFactory<TObject, TValue> : IValueObjectFactory<TObject, TValue>
    where TObject : notnull, ValueObject<TObject, TValue>
{
    protected ValueObjectFactory(IValueValidator<TValue>? validator = null, IEqualityComparer<TValue>? equalityComparer = null)
    {
        Validator = validator ?? ValueValidator<TValue>.None;
        EqualityComparer = equalityComparer ?? EqualityComparer<TValue>.Default;
    }

    protected IValueValidator<TValue> Validator { get; }

    protected IEqualityComparer<TValue> EqualityComparer { get; }

    public ErrorOr<TObject> Create(TValue value) => IsValid(value) ? CreateObject(value) : GetValidationError();

    protected virtual bool IsValid(TValue value) => Validator.IsValid(value);

    protected virtual ErrorOr<TObject> GetValidationError() => Error.Validation();

    protected abstract TObject CreateObject(TValue value);
}
