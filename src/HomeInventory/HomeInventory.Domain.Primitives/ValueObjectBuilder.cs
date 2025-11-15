using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : IValueObject<TObject>
    where TValue : notnull
{
    private Option<TValue> _value = Option<TValue>.None;

    public TSelf WithValue(TValue value)
    {
        _value = value;
        return (TSelf)this;
    }

    public Validation<Error, TObject> Build() =>
        _value
            .Map(Validate)
            .Map(validation => validation.Map(ToObject))
            .Match(Functional.Identity<Validation<Error, TObject>>(), static () => Statics.ValueNotSpecified);

    public void Reset() =>
        _value = Option<TValue>.None;

    protected abstract TObject ToObject(TValue value);

    protected virtual Validation<Error, TValue> Validate(TValue? value) =>
        value is null ? Statics.ValueNotSpecified : value;
}

file static class Statics
{
    public static Error ValueNotSpecified { get; } = new ValueNotSpecifiedError();
}
