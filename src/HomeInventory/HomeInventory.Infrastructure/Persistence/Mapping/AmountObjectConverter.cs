using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class AmountObjectConverter(IAmountFactory factory) : ObjectConverter<ProductAmountModel, Amount>
{
    private readonly IAmountFactory _factory = factory;

    public override Validation<Error, Amount> TryConvert(ProductAmountModel source)
    {
        var unit = AmountUnit.Parse(source.UnitName);
        return _factory.Create(source.Value, unit);
    }
}
