using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : BaseMappingsProfile
{
    public ContractsMappings()
    {
        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();
    }
}
