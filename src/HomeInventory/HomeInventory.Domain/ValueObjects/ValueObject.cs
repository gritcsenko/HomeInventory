namespace HomeInventory.Domain.ValueObjects;

public class ValueObject<TObject, TValue> : IValueObject<TObject, TValue>
    where TObject : notnull, ValueObject<TObject, TValue>
{
    private readonly TValue _value;
    private readonly IEqualityComparer<TValue> _equalityComparer;

    protected ValueObject(TValue value, IEqualityComparer<TValue> equalityComparer) => (_value, _equalityComparer) = (value, equalityComparer);

    public bool Equals(TObject? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    public override bool Equals(object? obj) => obj is TObject entity && Equals(entity);

    public override int GetHashCode() => _value is null ? 0 : _equalityComparer.GetHashCode(_value);

    protected virtual bool EqualsCore(TObject other) => _equalityComparer.Equals(_value, other._value);

    public static bool operator ==(ValueObject<TObject, TValue> a, ValueObject<TObject, TValue> b) => a.Equals(b);

    public static bool operator !=(ValueObject<TObject, TValue> a, ValueObject<TObject, TValue> b) => !a.Equals(b);

    public static explicit operator TValue(ValueObject<TObject, TValue> obj) => obj._value;
}
