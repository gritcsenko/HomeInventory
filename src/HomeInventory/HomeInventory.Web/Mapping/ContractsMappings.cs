using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using Mapster;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserId, Guid>().MapWith(x => x.Id);

        config.NewConfig<RegisterRequest, RegisterCommand>();
        config.NewConfig<RegistrationResult, RegisterResponse>();

        config.NewConfig<LoginRequest, AuthenticateQuery>();
        config.NewConfig<AuthenticateResult, LoginResponse>();
    }
}
