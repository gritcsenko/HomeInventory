using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using Mapster;

namespace HomeInventory.Api.Common.Mapping;

public class ContractsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();
        config.NewConfig<RegistrationResult, RegisterResponse>();

        config.NewConfig<LoginRequest, AuthenticateQuery>();
        config.NewConfig<AuthenticateResult, LoginResponse>();
    }
}
