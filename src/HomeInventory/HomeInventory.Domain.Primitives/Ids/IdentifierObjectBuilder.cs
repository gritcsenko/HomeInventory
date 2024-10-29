namespace HomeInventory.Domain.Primitives.Ids;

public abstract class IdentifierObjectBuilder<TSelf, TObject, TId>(Func<TId, TObject> objectFactoryFunc) : ValueObjectBuilder<TSelf, TObject, TId>
    where TSelf : IdentifierObjectBuilder<TSelf, TObject, TId>
    where TObject : class, IIdBuildable<TObject, TId, TSelf>, IBuildableIdentifierObject<TObject, TId, TSelf>, IValuableIdentifierObject<TObject, TId>
    where TId : notnull
{
    private readonly Func<TId, TObject> _objectFactoryFunc = objectFactoryFunc;

    protected IdentifierObjectBuilder()
        : this(TObject.CreateFrom)
    {
    }

    protected override Validation<Error, TId> Validate(TId? value) => IsIdValid(value) ? value : new ValidationError("Id has wrong value", value);

    protected override TObject ToObject(TId value) => _objectFactoryFunc(value);

    protected abstract bool IsIdValid(TId? value);
}
