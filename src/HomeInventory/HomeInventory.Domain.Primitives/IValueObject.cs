namespace HomeInventory.Domain.Primitives;

public interface IValueObject<TSelf> : IEquatable<TSelf>
    where TSelf : IValueObject<TSelf>
{
}
