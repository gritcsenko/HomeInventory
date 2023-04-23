namespace HomeInventory.Domain.Primitives;

public readonly struct EquatableComponent<T>
{
    private readonly IReadOnlyCollection<object> _components;

    public EquatableComponent(params object[] components) => _components = components;

    public override int GetHashCode() => _components.Aggregate(default(int), HashCode.Combine);

    public bool Equals(EquatableComponent<T> other) => _components.SequenceEqual(other._components);
}
