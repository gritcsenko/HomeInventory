namespace HomeInventory.Domain.Primitives.Ids;

public interface IIdentifierObject<TSelf> : IValueObject<TSelf>
    where TSelf : IIdentifierObject<TSelf>
{
}
