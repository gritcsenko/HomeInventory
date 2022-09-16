namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TObject> : IValueObject<TObject>
    where TObject : notnull, ValueObject<TObject>
{
    protected ValueObject()
    {
    }

    public static bool operator ==(ValueObject<TObject> left, ValueObject<TObject> right) => left.Equals(right);

    public static bool operator !=(ValueObject<TObject> left, ValueObject<TObject> right) => !left.Equals(right);

    public sealed override bool Equals(object? obj) => Equals(obj as TObject);

    public sealed override int GetHashCode() => GetHashCodeCore();

    public bool Equals(TObject? other) => other is TObject obj && (ReferenceEquals(obj, this) || EqualsCore(obj));

    protected virtual bool EqualsCore(TObject other) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    protected virtual int GetHashCodeCore() => GetEqualityComponents().Aggregate(new HashCode(), Combine).ToHashCode();

    protected abstract IEnumerable<object> GetEqualityComponents();

    private static HashCode Combine(HashCode hash, object obj)
    {
        hash.Add(obj);
        return hash;
    }
}
