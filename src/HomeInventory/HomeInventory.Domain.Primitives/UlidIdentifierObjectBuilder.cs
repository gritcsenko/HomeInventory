namespace HomeInventory.Domain.Primitives;

public sealed class UlidIdentifierObjectBuilder<TObject> : ValueObjectBuilder<UlidIdentifierObjectBuilder<TObject>, TObject, Ulid>
    where TObject : class, IUlidIdentifierObject<TObject>
{
    public UlidIdentifierObjectBuilder()
        : base(CreateInstance)
    {
    }

    private static TObject CreateInstance(Ulid value) =>
        ReflectionMethods.CreateInstance<TObject>(value)
            ?? throw new InvalidOperationException($"Got null instance during instance creation of {typeof(TObject).AssemblyQualifiedName}");

    public override bool IsValueValid<TSupplier>(in TSupplier value) => value.Invoke() != Ulid.Empty;
}
