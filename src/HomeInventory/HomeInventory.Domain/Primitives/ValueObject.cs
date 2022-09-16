using HomeInventory.Domain.Extensions;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TObject> : IValueObject<TObject>
    where TObject : notnull, ValueObject<TObject>
{
    private readonly Lazy<IEnumerable<object>> _lazyComponents;
    private readonly Lazy<int> _lazyHashCode;

    protected ValueObject()
    {
        _lazyComponents = LazyExtensions.ToLazy(GetAtomicComponentsCore, false);
        _lazyHashCode = LazyExtensions.ToLazy(GetHashCodeCore, false);
    }

    public static bool operator ==(ValueObject<TObject> left, ValueObject<TObject> right) => left.Equals(right);

    public static bool operator !=(ValueObject<TObject> left, ValueObject<TObject> right) => !left.Equals(right);

    public sealed override bool Equals(object? obj) => Equals(obj as TObject);

    public sealed override int GetHashCode() => _lazyHashCode.Value;

    public bool Equals(TObject? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    protected virtual bool EqualsCore(TObject other) => _lazyComponents.Value.SequenceEqual(other._lazyComponents.Value);

    protected virtual int GetHashCodeCore() => _lazyComponents.Value.Aggregate(default(int), HashCode.Combine);

    protected abstract IEnumerable<object> GetAtomicComponentsCore();
}
