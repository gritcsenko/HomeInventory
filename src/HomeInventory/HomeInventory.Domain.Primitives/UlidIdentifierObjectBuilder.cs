namespace HomeInventory.Domain.Primitives;

public sealed class UlidIdentifierObjectBuilder<TObject> : ValueObjectBuilder<UlidIdentifierObjectBuilder<TObject>, TObject, Ulid>
    where TObject : class, IUlidIdentifierObject<TObject>
{
    protected override bool IsValueValid(Ulid value) => value != Ulid.Empty;

    protected override Optional<TObject> ToObject(Ulid value)
    {
        var obj = ReflectionMethods.CreateInstance<TObject>(value);
        return obj is null ? Optional<TObject>.None : Optional.Some(obj);
    }
}
