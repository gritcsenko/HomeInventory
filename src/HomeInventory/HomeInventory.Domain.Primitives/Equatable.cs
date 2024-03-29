﻿namespace HomeInventory.Domain.Primitives;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4035:Classes implementing \"IEquatable<T>\" should be sealed", Justification = "Designed to be a base class")]
public abstract class Equatable<TSelf>(params object[] components) : IEquatable<TSelf>
    where TSelf : Equatable<TSelf>
{
    private readonly EquatableComponent<TSelf> _component = new(components);

    public static bool operator ==(Equatable<TSelf>? left, TSelf? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Equatable<TSelf>? left, TSelf? right) => !(left == right);

    public bool Equals(TSelf? other) => ReferenceEquals(other, this) || (other is not null && EqualsCore(other));

    public sealed override bool Equals(object? obj) => Equals(obj as TSelf);

    public sealed override int GetHashCode() => _component.GetHashCode();

    protected T GetComponent<T>(int index) => (T)GetComponent(index);

    protected object GetComponent(int index) => _component.GetComponent(index);

    private bool EqualsCore(TSelf other) => _component.Equals(other._component);
}
