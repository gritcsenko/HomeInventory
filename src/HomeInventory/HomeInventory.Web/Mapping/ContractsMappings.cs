using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Mapping;
using HomeInventory.Contracts;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : MappingProfile
{
    public ContractsMappings()
    {
        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();
    }
}
