using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Framework;

namespace HomeInventory.Web.UserManagement;

internal sealed class UserManagementContractsMappings : BaseMappingsProfile
{
    public UserManagementContractsMappings()
    {
        CreateMap<UserId>().Using(x => x.Value, UserId.Converter);
        CreateMap<Email>().Using(x => x.Value, x => new Email(x));

        CreateMap<RegisterRequest>().Using(CreateRegisterCommand);

        CreateMap<RegisterRequest>().To<UserIdQuery>();
        CreateMap<UserIdResult>().To<RegisterResponse>();
    }

    private static RegisterCommand CreateRegisterCommand(RegisterRequest c, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(c.Email);
        var password = c.Password;
        return new RegisterCommand(email, password);
    }
}
