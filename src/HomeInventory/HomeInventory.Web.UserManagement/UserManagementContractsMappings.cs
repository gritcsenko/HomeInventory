using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Domain.Primitives.Messages;
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

        CreateMap<RegisterRequest>().Using(CreateUserIdQuery);
        CreateMap<UserIdResult>().To<RegisterResponse>();
    }

    private static RegisterUserRequestMessage CreateRegisterCommand(RegisterRequest c, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(c.Email);
        var password = c.Password;
        var hub = (IMessageHub)ctx.State;
        return hub.CreateMessage((id, on) => new RegisterUserRequestMessage(id, on, email, password));
    }

    private static UserIdQueryMessage CreateUserIdQuery(RegisterRequest c, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(c.Email);
        var hub = (IMessageHub)ctx.State;
        return hub.CreateMessage((id, on) => new UserIdQueryMessage(id, on, email));
    }
}
