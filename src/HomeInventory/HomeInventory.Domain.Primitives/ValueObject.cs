using DotNext;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TObject> : Equatable<TObject>, IValueObject<TObject>
    where TObject : ValueObject<TObject>
{
    protected ValueObject(params object[] components)
        : base(components)
    {
    }

    public class Builder<TSelf, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
        where TSelf : Builder<TSelf, TValue>
        where TValue : notnull
    {
        private Optional<ISupplier<TValue>> _value = Optional.None<ISupplier<TValue>>();
        private readonly Func<TValue, TObject> _createFunc;

        public Builder(Func<TValue, TObject> createFunc) => _createFunc = createFunc;

        public TSelf WithValue<TSupplier>(in TSupplier value)
            where TSupplier : ISupplier<TValue>
        {
            _value = Optional.Some<ISupplier<TValue>>(value);
            return (TSelf)this;
        }

        public TObject Invoke() => _value.Convert(Create).OrThrow(() => new InvalidOperationException("value not provided"));

        public void Reset() => _value = Optional.None<ISupplier<TValue>>();

        private TObject Create(ISupplier<TValue> supplier) => _createFunc(supplier.Invoke());
    }

    public sealed class Builder<TValue> : Builder<Builder<TValue>, TValue>
        where TValue : notnull
    {
        public Builder(Func<TValue, TObject> createFunc)
            : base(createFunc)
        {
        }
    }
}
