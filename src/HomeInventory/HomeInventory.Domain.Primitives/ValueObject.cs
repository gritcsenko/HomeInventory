namespace HomeInventory.Domain.Primitives;

public abstract class ValueObject<TSelf>(params object[] components) : Equatable<TSelf>(components), IValueObject<TSelf>
    where TSelf : ValueObject<TSelf>
{
}
