using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives;

public class BuilderObjectConverter<TBuilder, TObject, TValue> : ObjectConverter<TValue, TObject>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : class, IValueObject<TObject>, IOptionalBuildable<TObject, TBuilder>
{
    protected sealed override OneOf<TObject, IError> TryConvertCore(TValue source) =>
        TObject
            .CreateBuilder()
            .WithValue(source)
            .Build()
            .Convert(obj => (OneOf<TObject, IError>)obj)
            .OrInvoke(() => new ObjectValidationError<TValue>(source));
}
