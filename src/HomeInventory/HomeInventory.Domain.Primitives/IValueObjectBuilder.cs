namespace HomeInventory.Domain.Primitives;

public interface IValueObjectBuilder<TSelf, TObject, in TValue> : IObjectBuilder<TObject>
    where TSelf : notnull, IValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    TSelf WithValue(TValue value);
    void Reset();
}
