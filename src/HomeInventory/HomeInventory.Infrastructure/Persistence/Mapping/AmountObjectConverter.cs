using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class AmountObjectConverter(IAmountFactory factory) : ObjectConverter<ProductAmountModel, Amount>
{
    private readonly IAmountFactory _factory = factory;

    protected override OneOf<Amount, IError> TryConvertCore(ProductAmountModel source) =>
        _factory.Create(source.Value, AmountUnit.Parse(source.UnitName));
}
