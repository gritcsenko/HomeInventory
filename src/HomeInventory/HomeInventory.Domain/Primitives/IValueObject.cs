namespace HomeInventory.Domain.Primitives;
public interface IValueObject<TObject> : IEquatable<TObject>
    where TObject : notnull, IValueObject<TObject>
{
}
