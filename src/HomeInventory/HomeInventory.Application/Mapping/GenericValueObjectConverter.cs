using AutoMapper;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Mapping;

public abstract class GenericValueObjectConverter<TObject, TValue> : IValueConverter<TValue, TObject>, ITypeConverter<TValue, TObject>
{
    public TObject Convert(TValue sourceMember, ResolutionContext context) => ConvertCore(sourceMember);

    public TObject Convert(TValue source, TObject destination, ResolutionContext context) => ConvertCore(source);

    public OneOf<TObject, IError> Convert(TValue source) => InternalConvert(source);

    private TObject ConvertCore(TValue source) =>
        InternalConvert(source)
        .Match(
            obj => obj,
            error => throw CreateException(error));

    protected abstract OneOf<TObject, IError> InternalConvert(TValue source);

    private static Exception CreateException(IError error)
    {
        var exception = new InvalidOperationException($"Cannot convert '{typeof(TValue).FullName}' to '{typeof(TObject).FullName}'. Reason: '{error.Message}'");
        foreach (var (key, value) in error.Metadata)
        {
            exception.Data.Add(key, value);
        }
        return exception;
    }
}
