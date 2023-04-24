namespace HomeInventory.Domain.Primitives;

public readonly struct EquatableComponent<T>
{
    private readonly IReadOnlyCollection<object> _components;

    public EquatableComponent()
        : this(Array.Empty<object>())
    {
    }

    public EquatableComponent(params object[] components) => _components = components;

    public override int GetHashCode() => _components.Aggregate(new HashCode(), Combine).ToHashCode();

    public bool Equals(EquatableComponent<T> other) => _components.SequenceEqual(other._components);

    private static HashCode Combine(HashCode hash, object obj)
    {
        hash.Add(obj);
        return hash;
    }
}
