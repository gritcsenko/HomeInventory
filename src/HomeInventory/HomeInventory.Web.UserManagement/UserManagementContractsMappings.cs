using AutoMapper;
using DotNext;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Framework;
using Visus.Cuid;

namespace HomeInventory.Web.UserManagement;

internal sealed class UserManagementContractsMappings : ContractsMappingProfile
{
    public UserManagementContractsMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForString(x => new Email(x), x => x.Value);

        CreateMap<RegisterRequest, RegisterCommand>()
            .ConstructUsing(CreateRegisterCommand);

        CreateMap<RegisterRequest, UserIdQuery>();
        CreateMap<UserIdResult, RegisterResponse>();
    }

    private static RegisterCommand CreateRegisterCommand(RegisterRequest c, ResolutionContext ctx)
    {
        var email = ctx.Mapper.MapOrFail<Email>(c.Email);
        var password = c.Password;
        var userIdSupplier = new DelegatingSupplier<Cuid>(Cuid.NewCuid);
        return new RegisterCommand(email, password, userIdSupplier);
    }
}
