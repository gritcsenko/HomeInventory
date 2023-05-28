namespace HomeInventory.Domain.Primitives;

public interface IIdentifierObject<TSelf> : IValueObject<TSelf>
    where TSelf : IIdentifierObject<TSelf>
{
}
