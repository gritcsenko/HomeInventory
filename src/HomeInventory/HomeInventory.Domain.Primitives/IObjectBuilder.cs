namespace HomeInventory.Domain.Primitives;

public interface IObjectBuilder<TObject>
    where TObject : notnull
{
    Validation<Error, TObject> Build();
}
