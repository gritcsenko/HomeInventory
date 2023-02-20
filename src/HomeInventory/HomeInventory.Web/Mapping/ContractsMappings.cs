using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Areas;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : MappingProfile
{
    public ContractsMappings()
    {
        CreateMapForId<UserId>();

        CreateMapForValue<Email, string>(x => x.Value);

        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<RegistrationResult, RegisterResponse>();

        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();

        CreateMap<AreasResult, AreaResponse[]>()
            .ConstructUsing(r => r.Areas.Select(a => new AreaResponse(a.Name.Value)).ToArray());
    }
}
