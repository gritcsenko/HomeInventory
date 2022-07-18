namespace HomeInventory.Domain.ValueObjects;
public interface IIdentifierObject<TObject> : IValueObject<TObject>
    where TObject : notnull, IIdentifierObject<TObject>
{
}
