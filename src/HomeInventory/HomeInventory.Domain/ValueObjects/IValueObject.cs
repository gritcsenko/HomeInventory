using System.Collections;

namespace HomeInventory.Domain.ValueObjects;

public interface IValueObject : IStructuralEquatable
{
}

public interface IValueObject<TObject> : IValueObject, IEquatable<TObject>
    where TObject : notnull, IValueObject<TObject>
{
}
