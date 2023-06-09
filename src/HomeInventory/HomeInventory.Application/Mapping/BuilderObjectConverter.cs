using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Mapping;

public class BuilderObjectConverter<TBuilder, TObject, TValue> : ObjectConverter<TObject, TValue>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : class, IValueObject<TObject>, IBuildable<TObject, TBuilder>
{
    protected sealed override OneOf<TObject, IError> TryConvertCore(TValue source)
    {
        var supplier = new ValueSupplier<TValue>(source);

#pragma warning disable CA2252 // This API requires opting into preview features
        var builder = TObject.CreateBuilder();
#pragma warning restore CA2252 // This API requires opting into preview features
        if (!builder.IsValueValid(supplier))
        {
            return new ObjectValidationError<TValue>(source);
        }

        builder.WithValue(supplier);
        return builder.Invoke();
    }
}
