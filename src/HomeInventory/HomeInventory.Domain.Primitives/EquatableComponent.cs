using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Domain.Primitives;

public readonly struct EquatableComponent<T>(IEnumerable<object> components) : IEquatable<EquatableComponent<T>>
{
    private readonly object[] _components = components as object[] ?? [.. components];

    public object GetComponent(int index) => _components[index];

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in _components)
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is EquatableComponent<T> component && Equals(component);

    public bool Equals(EquatableComponent<T> other) => _components.SequenceEqual(other._components);

    public static bool operator ==(EquatableComponent<T> left, EquatableComponent<T> right) => left.Equals(right);

    public static bool operator !=(EquatableComponent<T> left, EquatableComponent<T> right) => !(left == right);
}
