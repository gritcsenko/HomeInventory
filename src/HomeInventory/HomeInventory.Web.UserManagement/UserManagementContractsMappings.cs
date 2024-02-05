using DotNext;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Framework;

namespace HomeInventory.Web.UserManagement;

internal class UserManagementContractsMappings : ContractsMappingProfile
{
    public UserManagementContractsMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForString(x => new Email(x), x => x.Value);

        CreateMap<RegisterRequest, RegisterCommand>()
            .ConstructUsing((c, ctx) => new RegisterCommand(ctx.Mapper.MapOrFail<Email>(c.Email), c.Password, new DelegatingSupplier<Ulid>(Ulid.NewUlid)));

        CreateMap<RegisterRequest, UserIdQuery>();
        CreateMap<UserIdResult, RegisterResponse>();
    }
}
