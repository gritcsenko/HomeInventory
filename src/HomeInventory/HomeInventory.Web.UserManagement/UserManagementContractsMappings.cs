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

        CreateMap<RegisterRequest>().With<IMessageHubContext>()
            .Using(CreateRegisterCommand)
            .Using(CreateUserIdQuery);

        CreateMap<UserIdResult>().To<RegisterResponse>();
    }

    private static RegisterUserRequestMessage CreateRegisterCommand(RegisterRequest c, IMessageHubContext hubContext, IRuntimeMapper mapper)
    {
        var email = mapper.MapOrFail<Email>(c.Email);
        var password = c.Password;
        return hubContext.CreateMessage((id, on) => new RegisterUserRequestMessage(id, on, email, password));
    }

    private static UserIdQueryMessage CreateUserIdQuery(RegisterRequest c, IMessageHubContext hubContext, IRuntimeMapper mapper)
    {
        var email = mapper.MapOrFail<Email>(c.Email);
        return hubContext.CreateMessage((id, on) => new UserIdQueryMessage(id, on, email));
    }
}
