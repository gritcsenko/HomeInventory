namespace HomeInventory.Domain.Primitives;

public sealed class EquatableComponent<T>
    where T : notnull, IEquatable<T>
{
    private readonly IReadOnlyCollection<object> _components;

    public EquatableComponent(params object[] components) => _components = components;

    public sealed override bool Equals(object? obj) => obj is T t && Equals(t);

    public sealed override int GetHashCode() => _components.Aggregate(default(int), HashCode.Combine);

    public bool Equals(EquatableComponent<T>? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    private bool EqualsCore(EquatableComponent<T> other) => _components.SequenceEqual(other._components);
}
