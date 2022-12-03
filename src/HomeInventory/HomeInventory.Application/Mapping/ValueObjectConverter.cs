using AutoMapper;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class ValueObjectConverter<TObject, TValue> : IValueConverter<TValue, TObject>, ITypeConverter<TValue, TObject>
{
    private readonly IValueObjectFactory<TObject, TValue> _factory;

    public ValueObjectConverter(IValueObjectFactory<TObject, TValue> factory) => _factory = factory;

    public TObject Convert(TValue sourceMember, ResolutionContext context) => Convert(sourceMember);

    public TObject Convert(TValue source, TObject destination, ResolutionContext context) => Convert(source);

    private TObject Convert(TValue source)
    {
        var result = _factory.CreateFrom(source);
        return result.Match(
            obj => obj,
            error => throw new InvalidOperationException($"Cannot convert '{typeof(TValue).FullName}' to '{typeof(TObject).FullName}'. Reason: '{error.Message}'")
            );
    }
}
