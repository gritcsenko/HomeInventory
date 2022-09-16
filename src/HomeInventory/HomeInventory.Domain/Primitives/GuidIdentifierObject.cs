namespace HomeInventory.Domain.Primitives;

public abstract class GuidIdentifierObject<TObject> : ValueObject<TObject>, IIdentifierObject<TObject>
    where TObject : notnull, GuidIdentifierObject<TObject>
{
    protected GuidIdentifierObject(Guid value) => Value = value;

    protected Guid Value { get; }

    public Guid Id => Value;

    protected override IEnumerable<object> GetAtomicComponentsCore()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
