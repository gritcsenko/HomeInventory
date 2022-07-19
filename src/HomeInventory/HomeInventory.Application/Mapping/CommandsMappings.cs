using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using Mapster;

namespace HomeInventory.Application.Mapping;
internal class CommandsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterCommand, UserHasEmailSpecification>().MapWith(c => new UserHasEmailSpecification(c.Email));
        config.NewConfig<RegisterCommand, CreateUserSpecification>();
    }
}
