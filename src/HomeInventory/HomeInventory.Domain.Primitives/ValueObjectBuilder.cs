using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : IValueObject<TObject>
    where TValue : notnull
{
    private Optional<TValue> _value = None.Value;

    public TSelf WithValue(TValue value)
    {
        _value = value;
        return (TSelf)this;
    }

    public Validated<TObject, Error> Build() =>
        _value
            .Select(Validate)
            .Select(validation => validation.Select(ToObject))
            .IfNone(Statics.ValueNotSpecified);

    public void Reset() =>
        _value = None.Value;

    protected abstract TObject ToObject(TValue value);

    protected virtual Validated<TValue, Error> Validate(TValue? value) =>
        value is null ? Statics.ValueNotSpecified : value;
}

file static class Statics
{
    public static Error ValueNotSpecified { get; } = new ValueNotSpecifiedError();
}
