using AutoMapper;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class AmountObjectConverter(IAmountFactory factory) : ObjectConverter<Amount, ProductAmountModel>, ITypeConverter<ProductAmountModel, Amount>
{
    private readonly IAmountFactory _factory = factory;

    public Amount Convert(ProductAmountModel source, Amount destination, ResolutionContext context) => Convert(source);

    protected override OneOf<Amount, IError> TryConvertCore(ProductAmountModel source) =>
        _factory.Create(source.Value, AmountUnit.Parse(source.UnitName));
}
