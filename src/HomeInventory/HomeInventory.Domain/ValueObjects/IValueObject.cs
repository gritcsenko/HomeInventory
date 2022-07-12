namespace HomeInventory.Domain.ValueObjects;

public interface IValueObject
{
}

public interface IValueObject<TValue> : IValueObject
{
}

public interface IValueObject<TObject, TValue> : IValueObject<TValue>, IEquatable<TObject>
    where TObject : notnull, IValueObject<TObject, TValue>
{
}
