using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Domain.ValueObjects;

public abstract class ValueObject<TObject> : IValueObject<TObject>
    where TObject : notnull, ValueObject<TObject>
{
    protected ValueObject()
    {
    }

    public static bool operator ==(ValueObject<TObject> left, ValueObject<TObject> right) => left.Equals(right);

    public static bool operator !=(ValueObject<TObject> left, ValueObject<TObject> right) => !left.Equals(right);

    public sealed override bool Equals(object? obj) => Equals(obj, EqualityComparer<object>.Default);

    public sealed override int GetHashCode() => GetHashCodeCore(Adapt(EqualityComparer<object>.Default));

    public bool Equals(object? other, IEqualityComparer comparer) => ReferenceEquals(other, this) || (other is TObject obj && EqualsCore(obj, Adapt(comparer)));

    public int GetHashCode(IEqualityComparer comparer) => GetHashCodeCore(Adapt(comparer));

    public bool Equals(TObject? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other, EqualityComparer<object>.Default));

    protected virtual bool EqualsCore(TObject other, IEqualityComparer<object> comparer) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents(), comparer);

    protected virtual int GetHashCodeCore(IEqualityComparer<object> comparer)
    {
        return GetEqualityComponents().Aggregate(new HashCode(), (h, o) => Combine(h, o, comparer)).ToHashCode();
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    private static HashCode Combine<T>(HashCode hash, T obj, IEqualityComparer<T> comparer)
    {
        hash.Add(obj, comparer);
        return hash;
    }

    private static IEqualityComparer<object> Adapt(IEqualityComparer comparer) => comparer as IEqualityComparer<object> ?? new EqualityComparerAdapter(comparer);

    private class EqualityComparerAdapter : IEqualityComparer<object>
    {
        private readonly IEqualityComparer _comparer;

        public EqualityComparerAdapter(IEqualityComparer comparer) => _comparer = comparer;

        public new bool Equals(object? x, object? y) => _comparer.Equals(x, y);

        public int GetHashCode([DisallowNull] object obj) => _comparer.GetHashCode(obj);
    }
}
