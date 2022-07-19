namespace HomeInventory.Domain.ValueObjects;
public interface IValueObject<TObject> : IEquatable<TObject>
    where TObject : notnull, IValueObject<TObject>
{
}
