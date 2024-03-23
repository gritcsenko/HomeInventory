﻿namespace HomeInventory.Domain.Primitives;

public abstract class UlidIdentifierObject<TSelf>(Ulid value) : ValueObject<TSelf>(value), IUlidIdentifierObject<TSelf>
    where TSelf : UlidIdentifierObject<TSelf>
{
    public Ulid Value { get; } = value;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static UlidIdentifierObjectBuilder<TSelf> CreateBuilder() => new();

    public override string ToString() => Value.ToString();
}
