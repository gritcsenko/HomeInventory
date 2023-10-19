namespace HomeInventory.Domain.Primitives;

public sealed class UlidIdentifierObjectBuilder<TObject> : ValueObjectBuilder<UlidIdentifierObjectBuilder<TObject>, TObject, Ulid>
    where TObject : class, IUlidBuildable<TObject>, IUlidIdentifierObject<TObject>
{
    private readonly Func<Ulid, Result<TObject>> _objectFactoryFunc;

    public UlidIdentifierObjectBuilder()
        : this(TObject.CreateFrom)
    {
    }

    public UlidIdentifierObjectBuilder(Func<Ulid, Result<TObject>> objectFactoryFunc) =>
        _objectFactoryFunc = objectFactoryFunc;

    protected override bool IsValueValid(Ulid value) => value != Ulid.Empty;

    protected override Optional<TObject> ToObject(Ulid value)
    {
        var result = _objectFactoryFunc(value);
        return result.TryGet();
    }
}
