namespace HomeInventory.Domain.Primitives;

public interface IValueObjectBuilder<out TSelf, TObject, in TValue> : IObjectBuilder<TObject>
    where TSelf : IValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : IValueObject<TObject>
    where TValue : notnull
{
    TSelf WithValue(TValue value);
    void Reset();
}
