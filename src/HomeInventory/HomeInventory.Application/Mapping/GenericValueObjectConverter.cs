using AutoMapper;
using FluentResults;
using OneOf;

namespace HomeInventory.Application.Mapping;

public abstract class GenericValueObjectConverter<TObject, TValue> : IValueConverter<TValue, TObject>, ITypeConverter<TValue, TObject>
{
    public TObject Convert(TValue sourceMember, ResolutionContext context) => Convert(sourceMember);

    public TObject Convert(TValue source, TObject destination, ResolutionContext context) => Convert(source);

    private TObject Convert(TValue source)
    {
        var result = InternalConvert(source);
        return result.Match(
            obj => obj,
            error => throw new InvalidOperationException($"Cannot convert '{typeof(TValue).FullName}' to '{typeof(TObject).FullName}'. Reason: '{error.Message}'"));
    }

    protected abstract OneOf<TObject, IError> InternalConvert(TValue source);
}
