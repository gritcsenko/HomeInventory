using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class ModelMappings : BaseMappingsProfile
{
    public ModelMappings()
    {
        CreateMap<StorageAreaId>().Using(x => x.Value, StorageAreaId.Converter);
        CreateMap<ProductId>().Using(x => x.Value, ProductId.Converter);
        CreateMap<MaterialId>().Using(x => x.Value, MaterialId.Converter);

        CreateMap<string>().Using<StorageAreaName>(x => new(x), x => x.Value);

        CreateMap<StorageArea, StorageAreaModel>().ReverseMap();
        CreateMap<Product, ProductModel>().ReverseMap();
        CreateMap<Amount>().Using<ProductAmountModel, AmountObjectConverter>(obj => new() { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
