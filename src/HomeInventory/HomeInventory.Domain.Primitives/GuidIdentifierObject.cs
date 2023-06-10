using System.Runtime.Versioning;

namespace HomeInventory.Domain.Primitives;

public abstract class GuidIdentifierObject<TSelf> : ValueObject<TSelf>, IGuidIdentifierObject<TSelf>
    where TSelf : GuidIdentifierObject<TSelf>
{
    protected GuidIdentifierObject(Guid value)
        : base(value)
    {
        Id = value;
    }

    public Guid Id { get; }

    [RequiresPreviewFeatures]
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static GuidIdentifierObjectBuilder<TSelf> CreateBuilder() => new();
#pragma warning restore CA1000 // Do not declare static members on generic types

    public override string ToString() => Id.ToString();
}
