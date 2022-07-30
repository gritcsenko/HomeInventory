using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using Mapster;

namespace HomeInventory.Application.Mapping;

internal class QueryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuthenticateQuery, FilterSpecification<User>>().MapWith(c => UserSpecifications.HasEmail(c.Email));
    }
}
