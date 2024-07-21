namespace HomeInventory.Domain.Primitives.Ids;

public abstract class IdentifierObjectBuilder<TSelf, TObject, TId>(Func<TId, Optional<TObject>> objectFactoryFunc) : ValueObjectBuilder<TSelf, TObject, TId>
    where TSelf : IdentifierObjectBuilder<TSelf, TObject, TId>
    where TObject : class, IIdBuildable<TObject, TId, TSelf>, IBuildableIdentifierObject<TObject, TId, TSelf>, IValuableIdentifierObject<TObject, TId>
    where TId : notnull
{
    private readonly Func<TId, Optional<TObject>> _objectFactoryFunc = objectFactoryFunc;

    protected IdentifierObjectBuilder()
        : this(TObject.CreateFrom)
    {
    }

    protected override bool IsValueValid(TId value) => IsIdValid(value);

    protected override Optional<TObject> ToObject(TId value) => _objectFactoryFunc(value);

    protected abstract bool IsIdValid(TId value);
}
