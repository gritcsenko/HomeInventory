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
    private readonly Func<TValue, bool> _isValid;

    public BuilderObjectConverter(Func<TValue, bool> isValid)
    {
        _isValid = isValid;
    }

    protected sealed override OneOf<TObject, IError> TryConvertCore(TValue source)
    {
        if (!_isValid(source))
        {
            return new ObjectValidationError<TValue>(source);
        }

#pragma warning disable CA2252 // This API requires opting into preview features
        var builder = TObject.CreateBuilder();
#pragma warning restore CA2252 // This API requires opting into preview features
        builder.WithValue(new ValueSupplier<TValue>(source));
        return builder.Invoke();
    }
}
