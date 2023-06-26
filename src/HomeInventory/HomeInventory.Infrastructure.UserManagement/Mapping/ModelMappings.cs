using HomeInventory.Domain.Aggregates;
using HomeInventory.Infrastructure.Framework.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.UserManagement.Mapping;

internal class ModelMappings : ModelMappingsProfile
{
    public ModelMappings()
    {
        CreateMap<User, UserModel>().ReverseMap();
    }
}
