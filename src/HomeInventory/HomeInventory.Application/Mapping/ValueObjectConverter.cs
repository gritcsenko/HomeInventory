using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using OneOf;

namespace HomeInventory.Application.Mapping;

public class ValueObjectConverter<TObject, TValue> : GenericValueObjectConverter<TObject, TValue>
{
    private readonly IValueObjectFactory<TObject, TValue> _factory;

    public ValueObjectConverter(IValueObjectFactory<TObject, TValue> factory) => _factory = factory;

    protected override OneOf<TObject, IError> InternalConvert(TValue source) => _factory.CreateFrom(source);
}
