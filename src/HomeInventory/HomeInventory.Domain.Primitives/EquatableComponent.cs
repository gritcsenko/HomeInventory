using System.Diagnostics.CodeAnalysis;
using DotNext.Runtime;

namespace HomeInventory.Domain.Primitives;

public readonly struct EquatableComponent<T> : IEquatable<EquatableComponent<T>>
{
    private readonly object[] _components;
    private readonly int _hashCode;

    public EquatableComponent(object[] components)
    {
        _components = components;
        var hash = new HashCode();
        for (int i = 0; i < Intrinsics.GetLength(components); i++)
        {
            var component = components[i];
            hash.Add(component);
        }

        _hashCode = hash.ToHashCode();
    }

    public object GetComponent(int index) => _components[index];

    public override int GetHashCode() => _hashCode;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is EquatableComponent<T> component && Equals(component);

    public bool Equals(EquatableComponent<T> other) => _hashCode == other._hashCode && _components.SequenceEqual(other._components);

    public static bool operator ==(EquatableComponent<T> left, EquatableComponent<T> right) => left.Equals(right);

    public static bool operator !=(EquatableComponent<T> left, EquatableComponent<T> right) => !(left == right);
}
