using System.Runtime.Versioning;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Mapping;

public class BuilderObjectConverter<TBuilder, TObject, TValue> : ObjectConverter<TObject, TValue>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : class, IValueObject<TObject>, IBuildable<TObject, TBuilder>
{
    [RequiresPreviewFeatures]
    protected sealed override OneOf<TObject, IError> TryConvertCore(TValue source)
    {
        var supplier = new ValueSupplier<TValue>(source);

        var builder = TObject.CreateBuilder();
        if (!builder.IsValueValid(supplier))
        {
            return new ObjectValidationError<TValue>(source);
        }

        builder.WithValue(supplier);
        return builder.Invoke();
    }
}
