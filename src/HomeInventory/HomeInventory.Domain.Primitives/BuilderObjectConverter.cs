namespace HomeInventory.Domain.Primitives;

public class BuilderObjectConverter<TBuilder, TObject, TValue> : ObjectConverter<TValue, TObject>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : class, IValueObject<TObject>, IBuildableObject<TObject, TBuilder>
{
    public sealed override Validation<Error, TObject> TryConvert(TValue source) =>
        TObject
            .CreateBuilder()
            .WithValue(source)
            .Build();
}
