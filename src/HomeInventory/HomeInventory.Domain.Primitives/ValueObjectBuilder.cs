namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    private Optional<TValue> _value = Optional.None<TValue>();

    protected TSelf This => (TSelf)this;

    public TSelf WithValue(TValue value)
    {
        _value = Optional.Some(value);
        return (TSelf)this;
    }

    public Optional<TObject> Invoke() =>
        _value
            .If(IsValueValid)
            .Convert(ToObject);

    public void Reset() =>
        _value = Optional.None<TValue>();

    protected abstract Optional<TObject> ToObject(TValue value);

    protected virtual bool IsValueValid(TValue value) =>
        value is not null;
}
