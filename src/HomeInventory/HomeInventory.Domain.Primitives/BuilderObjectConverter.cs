using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Mapping;

public class BuilderObjectConverter<TBuilder, TObject, TValue> : ObjectConverter<TObject, TValue>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : class, IValueObject<TObject>, IOptionalBuildable<TObject, TBuilder>
{
    protected sealed override OneOf<TObject, IError> TryConvertCore(TValue source) =>
        TObject
            .CreateBuilder()
            .WithValue(source)
            .Invoke()
            .Convert(obj => (OneOf<TObject, IError>)obj)
            .OrInvoke(() => new ObjectValidationError<TValue>(source));
}
