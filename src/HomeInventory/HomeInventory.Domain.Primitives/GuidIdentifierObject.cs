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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static GuidIdentifierObjectBuilder<TSelf> CreateBuilder() => new();

    public override string ToString() => Id.ToString();
}
