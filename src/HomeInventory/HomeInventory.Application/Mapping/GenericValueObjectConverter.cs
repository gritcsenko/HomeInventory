using AutoMapper;
using DotNext;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Mapping;

public abstract class GenericValueObjectConverter<TObject, TValue> : IValueConverter<TValue, TObject>, ITypeConverter<TValue, TObject>
{
    public TObject Convert(TValue sourceMember, ResolutionContext context) => Convert(sourceMember);

    public TObject Convert(TValue source, TObject destination, ResolutionContext context) => Convert(source);

    public TObject Convert(TValue source) =>
        TryConvert(source)
            .Match(Func.Identity<TObject>(), error => throw CreateException(error));

    public OneOf<TObject, IError> TryConvert(TValue source) => TryConvertCore(source);

    protected abstract OneOf<TObject, IError> TryConvertCore(TValue source);

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
