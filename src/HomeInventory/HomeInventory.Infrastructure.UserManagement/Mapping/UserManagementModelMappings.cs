using HomeInventory.Domain.Aggregates;
using HomeInventory.Infrastructure.Framework.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.UserManagement.Mapping;

internal sealed class UserManagementModelMappings : ModelMappingsProfile
{
    public UserManagementModelMappings()
    {
        CreateMap<User, UserModel>().ReverseMap();
    }
}
