using DotNext;

namespace HomeInventory.Domain.Primitives;

public sealed class GuidIdentifierObjectBuilder<TObject> : ValueObjectBuilder<GuidIdentifierObjectBuilder<TObject>, TObject, Guid>
    where TObject : class, IGuidIdentifierObject<TObject>
{
    private static readonly ISupplier<Guid> _newSupplier = new DelegatingSupplier<Guid>(Guid.NewGuid);

    public GuidIdentifierObjectBuilder()
        : base(CreateInstance)
    {
    }

    private static TObject CreateInstance(Guid value) =>
        TypeExtensions.CreateInstance<TObject>(value)
            ?? throw new InvalidOperationException($"Got null instance during instance creation of {typeof(TObject).AssemblyQualifiedName}");

    public GuidIdentifierObjectBuilder<TObject> WithNewValue() => WithValue(_newSupplier);

    public static bool IsValid(Guid value) => value != Guid.Empty;
}
