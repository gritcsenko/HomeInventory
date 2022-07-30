using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using Mapster;

namespace HomeInventory.Application.Mapping;
internal class CommandsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterCommand, FilterSpecification<User>>().MapWith(c => UserSpecifications.HasEmail(c.Email));
        config.NewConfig<RegisterCommand, CreateUserSpecification>();
    }
}
