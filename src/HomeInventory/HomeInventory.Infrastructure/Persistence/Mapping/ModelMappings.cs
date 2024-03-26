using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class ModelMappings : MappingProfile
{
    public ModelMappings()
    {
        CreateMapForId<StorageAreaId>();
        CreateMapForId<ProductId>();
        CreateMapForId<MaterialId>();

        CreateMapForString(x => new StorageAreaName(x), x => x.Value);

        CreateMap<StorageArea, StorageAreaModel>().ReverseMap();
        CreateMap<Product, ProductModel>().ReverseMap();
        CreateMapForValue<Amount, ProductAmountModel, AmountObjectConverter>(obj => new ProductAmountModel { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
