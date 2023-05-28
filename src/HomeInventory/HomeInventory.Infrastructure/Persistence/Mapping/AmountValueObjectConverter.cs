using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class AmountValueObjectConverter : GenericValueObjectConverter<Amount, ProductAmountModel>
{
    private readonly IAmountFactory _factory;

    public AmountValueObjectConverter(IAmountFactory factory) => _factory = factory;

    protected override OneOf<Amount, IError> TryConvertCore(ProductAmountModel source) =>
        _factory.Create(source.Value, AmountUnit.Parse(source.UnitName));
}
