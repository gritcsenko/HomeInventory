namespace HomeInventory.Domain.Primitives;

public sealed class GuidIdentifierObjectBuilder<TObject> : ValueObjectBuilder<GuidIdentifierObjectBuilder<TObject>, TObject, Guid>
    where TObject : class, IGuidIdentifierObject<TObject>
{
    public GuidIdentifierObjectBuilder()
        : base(CreateInstance)
    {
    }

    private static TObject CreateInstance(Guid value) =>
        ReflectionMethods.CreateInstance<TObject>(value)
            ?? throw new InvalidOperationException($"Got null instance during instance creation of {typeof(TObject).AssemblyQualifiedName}");

    public static bool IsValid(Guid value) => value != Guid.Empty;
}
