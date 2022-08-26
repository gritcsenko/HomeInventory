namespace HomeInventory.Domain.Primitives;

public interface IIdentifierObject<TObject> : IValueObject<TObject>
    where TObject : notnull, IIdentifierObject<TObject>
{
}
