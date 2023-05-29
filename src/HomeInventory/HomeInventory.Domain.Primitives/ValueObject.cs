namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TSelf> : Equatable<TSelf>, IValueObject<TSelf>
    where TSelf : ValueObject<TSelf>
{
    protected ValueObject(params object[] components)
        : base(components)
    {
    }
}
