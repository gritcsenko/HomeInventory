using AutoMapper;
using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Web.Framework;

namespace HomeInventory.Web.UserManagement;

internal sealed class UserManagementContractsMappings : BaseMappingsProfile
{
    public UserManagementContractsMappings()
    {
        CreateMap<UserId>().Using(x => x.Value, UserId.Converter);
        CreateMap<Email>().Using(x => x.Value, x => new(x));

        CreateMap<RegisterRequest>().Using(CreateRegisterCommand);

        CreateMap<RegisterRequest>().To<UserIdQuery>();
        CreateMap<UserIdResult>().To<RegisterResponse>();
    }

    private static RegisterCommand CreateRegisterCommand(RegisterRequest c, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(c.Email);
        var password = c.Password;
        return new(email, password);
    }
}
