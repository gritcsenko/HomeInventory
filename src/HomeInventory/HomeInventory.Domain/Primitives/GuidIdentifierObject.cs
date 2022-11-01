namespace HomeInventory.Domain.Primitives;

public abstract class GuidIdentifierObject<TObject> : ValueObject<TObject>, IIdentifierObject<TObject>
    where TObject : GuidIdentifierObject<TObject>
{
    protected GuidIdentifierObject(Guid value)
        : base(value)
    {
        Id = value;
    }

    public Guid Id { get; }

    public override string ToString() => Id.ToString();
}
