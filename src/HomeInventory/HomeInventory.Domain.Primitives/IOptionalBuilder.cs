namespace HomeInventory.Domain.Primitives;

public interface IOptionalBuilder<TObject>
    where TObject : notnull
{
    Optional<TObject> Build();
}
