namespace HomeInventory.Domain.Primitives.Ids;

public abstract class IdentifierObjectBuilder<TSelf, TObject, TId>(Func<TId, Result<TObject>> objectFactoryFunc) : ValueObjectBuilder<TSelf, TObject, TId>
    where TSelf : IdentifierObjectBuilder<TSelf, TObject, TId>
    where TObject : class, IIdBuildable<TObject, TId, TSelf>, IBuildableIdentifierObject<TObject, TId, TSelf>, IValuableIdentifierObject<TObject, TId>
    where TId : notnull
{
    private readonly Func<TId, Result<TObject>> _objectFactoryFunc = objectFactoryFunc;

    protected IdentifierObjectBuilder()
        : this(TObject.CreateFrom)
    {
    }

    protected override bool IsValueValid(TId value) => IsIdValid(value);

    protected override Optional<TObject> ToObject(TId value)
    {
        var result = _objectFactoryFunc(value);
        return result.TryGet();
    }

    protected abstract bool IsIdValid(TId value);
}
