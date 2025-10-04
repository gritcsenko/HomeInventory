using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Infrastructure.UserManagement.Models;

namespace HomeInventory.Infrastructure.UserManagement.Mapping;

internal sealed class UserManagementModelMappings : BaseMappingsProfile
{
    public UserManagementModelMappings() => CreateMap<User, UserModel>().ReverseMap();
}
