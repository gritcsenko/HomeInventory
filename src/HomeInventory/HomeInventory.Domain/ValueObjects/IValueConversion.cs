namespace HomeInventory.Domain.ValueObjects;

public interface IValueConversion<TSelf, TValue>
    where TSelf : IValueConversion<TSelf, TValue>
{
    static abstract explicit operator TValue(TSelf obj);
}
