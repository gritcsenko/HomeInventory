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
        CreateMap<LoginRequest>().Using(CreateAuthenticateRequestMessage);
        CreateMap<AuthenticateResult, LoginResponse>();
    }

    private AuthenticateRequestMessage CreateAuthenticateRequestMessage(LoginRequest request, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(request.Email);
        var password = request.Password;
        var hub = (IMessageHub)ctx.State;
        return hub.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, email, password));
    }
}
