using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class ModelMappings : BaseMappingsProfile
{
    public ModelMappings()
    {
        CreateMap<ProductId>().Using(static x => x.Value, ProductId.Converter);
        CreateMap<Amount>().Using<ProductAmountModel, AmountObjectConverter>(static obj => new() { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
