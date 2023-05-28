using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class ModelMappings : MappingProfile
{
    public ModelMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForId<ProductId>();

        CreateMapForString(x => new Email(x), x => x.Value);

        CreateMap<User, UserModel>().ReverseMap();

        CreateMapForValue<Amount, ProductAmountModel, AmountObjectConverter>(obj => new ProductAmountModel { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
