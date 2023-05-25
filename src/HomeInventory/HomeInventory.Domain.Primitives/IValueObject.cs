namespace HomeInventory.Domain.Primitives;

public interface IValueObject
{
}

public interface IValueObject<TObject> : IValueObject, IEquatable<TObject>
    where TObject : IValueObject<TObject>
{
}
