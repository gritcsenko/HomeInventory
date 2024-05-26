using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.UserManagement.Mapping;

internal sealed class UserManagementModelMappings : BaseMappingsProfile
{
    public UserManagementModelMappings()
    {
        CreateMap<User, UserModel>().ReverseMap();
    }
}
