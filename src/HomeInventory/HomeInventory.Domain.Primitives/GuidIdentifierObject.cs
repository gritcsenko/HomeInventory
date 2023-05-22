using DotNext;

namespace HomeInventory.Domain.Primitives;

public abstract class GuidIdentifierObject<TObject> : ValueObject<TObject>, IIdentifierObject<TObject>
    where TObject : notnull, GuidIdentifierObject<TObject>
{
    protected GuidIdentifierObject(Guid value)
        : base(value)
    {
        Id = value;
    }

    public Guid Id { get; }

    public override string ToString() => Id.ToString();

    public sealed class Builder : Builder<Builder, Guid>
    {
        public Builder(Func<Guid, TObject> createFunc)
            : base(createFunc)
        {
        }

        public Builder WithNewValue() => WithValue(new DelegatingSupplier<Guid>(Guid.NewGuid));
    }
}
