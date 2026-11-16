namespace HomeInventory.Domain.Primitives;

public interface IObjectBuilder<TObject>
    where TObject : notnull
{
    Validated<TObject, Error> Build();
}
