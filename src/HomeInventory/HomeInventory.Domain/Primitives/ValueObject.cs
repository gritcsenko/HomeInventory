namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TObject> : Equatable<TObject>, IValueObject<TObject>
    where TObject : ValueObject<TObject>
{
    protected ValueObject(params object[] components)
        : base(components)
    {
    }
}
