using System.Collections;

namespace HomeInventory.Domain.Primitives;

public interface IValueObject : IStructuralEquatable
{
}

public interface IValueObject<TObject> : IValueObject, IEquatable<TObject>
    where TObject : notnull, IValueObject<TObject>
{
}
