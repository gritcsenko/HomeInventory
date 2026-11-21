namespace HomeInventory.Core;

public readonly struct None : IEquatable<None>
{
    public static None Value => default;

    public bool Equals(None other) => true;

    public override bool Equals(object? obj) => obj is None;

    public override int GetHashCode() => 0;

    public static bool operator ==(None left, None right) => left.Equals(right);

    public static bool operator !=(None left, None right) => !(left == right);
}
