using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using Mapster;

namespace HomeInventory.Application.Mapping;

internal class QueryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuthenticateQuery, UserHasEmailSpecification>().MapWith(c => new UserHasEmailSpecification(c.Email));
    }
}
