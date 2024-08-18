using AutoMapper;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Framework;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : BaseMappingsProfile
{
    public ContractsMappings()
    {
        CreateMap<LoginRequest>().With<IMessageHubContext>().Using(CreateAuthenticateRequestMessage);
        CreateMap<AuthenticateResult, LoginResponse>();
    }

    private static AuthenticateRequestMessage CreateAuthenticateRequestMessage(LoginRequest request, IMessageHubContext hubContext, IRuntimeMapper mapper)
    {
        var email = mapper.MapOrFail<Email>(request.Email);
        var password = request.Password;
        return hubContext.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, email, password));
    }
}

public sealed class LoginRequestConverter : ITypeConverter<LoginRequest, AuthenticateRequestMessage>
{
    public AuthenticateRequestMessage Convert(LoginRequest source, AuthenticateRequestMessage destination, ResolutionContext context)
    {
        throw new NotImplementedException();
    }
}
