using DotNext;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Application.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : MappingProfile
{
    public ContractsMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForString(x => new Email(x), x => x.Value);

        CreateMap<RegisterRequest, RegisterCommand>()
            .ConstructUsing((c, ctx) => new RegisterCommand(ctx.Mapper.Map<Email>(c.Email), c.Password, new DelegatingSupplier<Guid>(Guid.NewGuid)));

        CreateMap<RegisterRequest, UserIdQuery>();
        CreateMap<UserIdResult, RegisterResponse>();

        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();

        CreateMap<AreasResult, AreaResponse[]>()
            .ConstructUsing(r => r.Areas.Select(a => new AreaResponse(a.Name.Value)).ToArray());
    }
}
