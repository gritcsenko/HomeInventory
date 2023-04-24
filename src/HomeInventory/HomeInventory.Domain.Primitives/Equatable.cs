namespace HomeInventory.Domain.Primitives;

public abstract class Equatable<T> : IEquatable<T>
    where T : Equatable<T>
{
    private readonly EquatableComponent<T> _component;

    protected Equatable(params object[] components) => _component = new EquatableComponent<T>(components);

    public static bool operator ==(Equatable<T>? left, T? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Equatable<T>? left, T? right) => !(left == right);

    public sealed override bool Equals(object? obj) => Equals(obj as T);

    public sealed override int GetHashCode() => _component.GetHashCode();

    public bool Equals(T? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    private bool EqualsCore(T other) => _component.Equals(other._component);
}
