namespace HomeInventory.Domain.Primitives;

public interface IValueObject
{
}

public interface IValueObject<TSelf> : IValueObject, IEquatable<TSelf>
    where TSelf : IValueObject<TSelf>
{
}
