namespace HomeInventory.Domain.Primitives;

public abstract class Equatable<T> : IEquatable<T>
    where T : Equatable<T>
{
    private readonly IReadOnlyCollection<object> _components;

    protected Equatable(params object[] components) => _components = components;

    public static bool operator ==(Equatable<T>? left, T? right) => EqualOperator(left, right);

    public static bool operator !=(Equatable<T>? left, T? right) => !(left == right);

    public sealed override bool Equals(object? obj) => Equals(obj as T);

    public sealed override int GetHashCode() => _components.Aggregate(default(int), HashCode.Combine);

    public bool Equals(T? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    private static bool EqualOperator(Equatable<T>? left, T? right) => !(left is null ^ right is null) && left?.Equals(right!) != false;

    private bool EqualsCore(T other) => _components.SequenceEqual(other._components);
}
