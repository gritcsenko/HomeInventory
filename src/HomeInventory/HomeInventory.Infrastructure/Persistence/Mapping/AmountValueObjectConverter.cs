using FluentResults;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class AmountValueObjectConverter : GenericValueObjectConverter<Amount, ProductAmountModel>
{
    private readonly IAmountFactory _factory;

    public AmountValueObjectConverter(IAmountFactory factory)
    {
        _factory = factory;
    }

    protected override OneOf<Amount, IError> InternalConvert(ProductAmountModel source)
    {
        var unit = AmountUnit.TryParse(source.UnitName).Match(
                    x => x,
                    notFound => throw new InvalidOperationException("")
                );
        return _factory.Create(source.Value, unit);
    }
}
