using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class ModelMappings : MappingProfile
{
    public ModelMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForId<StorageAreaId>();
        CreateMapForId<ProductId>();

        CreateMapForString(x => new Email(x), x => x.Value);
        CreateMapForString(x => new StorageAreaName(x), x => x.Value);

        CreateMap<User, UserModel>().ReverseMap();

        CreateMap<StorageArea, StorageAreaModel>().ReverseMap();
        CreateMap<Product, ProductModel>().ReverseMap();
        CreateMapForValue<Amount, ProductAmountModel, AmountObjectConverter>(obj => new ProductAmountModel { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
