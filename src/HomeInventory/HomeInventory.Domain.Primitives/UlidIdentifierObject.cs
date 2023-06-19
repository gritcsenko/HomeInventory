using System.Runtime.Versioning;

namespace HomeInventory.Domain.Primitives;

public abstract class UlidIdentifierObject<TSelf> : ValueObject<TSelf>, IUlidIdentifierObject<TSelf>
    where TSelf : UlidIdentifierObject<TSelf>
{
    protected UlidIdentifierObject(Ulid value)
        : base(value)
    {
        Value = value;
    }

    public Ulid Value { get; }

    [RequiresPreviewFeatures]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static UlidIdentifierObjectBuilder<TSelf> CreateBuilder() => new();

    public override string ToString() => Value.ToString();
}
