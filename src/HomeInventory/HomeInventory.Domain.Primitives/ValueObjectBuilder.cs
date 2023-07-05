namespace HomeInventory.Domain.Primitives;

public class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    private Optional<ISupplier<TValue>> _value = Optional.None<ISupplier<TValue>>();
    private readonly Func<TValue, TObject> _createFunc;

    public ValueObjectBuilder(Func<TValue, TObject> createFunc) => _createFunc = createFunc;

    protected TSelf This => (TSelf)this;

    public virtual bool IsValueValid<TSupplier>(in TSupplier value)
        where TSupplier : ISupplier<TValue> =>
        true;

    public TSelf WithValue<TSupplier>(in TSupplier value)
        where TSupplier : ISupplier<TValue>
    {
        _value = Optional.Some<ISupplier<TValue>>(value);
        return This;
    }

    public TObject Invoke() =>
        _value
            .Convert(supplier => _createFunc(supplier.Invoke()))
            .OrThrow(() => new InvalidOperationException("value not provided"));

    public void Reset() =>
        _value = Optional.None<ISupplier<TValue>>();
}

public sealed class ValueObjectBuilder<TObject, TValue> : ValueObjectBuilder<ValueObjectBuilder<TObject, TValue>, TObject, TValue>
    where TObject : IValueObject<TObject>
    where TValue : notnull
{
    public ValueObjectBuilder(Func<TValue, TObject> createFunc)
        : base(createFunc)
    {
    }
}
