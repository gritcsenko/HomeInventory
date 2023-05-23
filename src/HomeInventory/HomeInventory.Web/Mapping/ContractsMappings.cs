﻿using DotNext;
using HomeInventory.Application.Cqrs.Commands.Register;
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
        CreateMapForString<Email>(x => x.Value);

        CreateMap<RegisterRequest, RegisterCommand>();

        CreateMap<RegisterRequest, RegisterCommand>()
            .ConstructUsing((c, ctx) => new RegisterCommand(ctx.Mapper.Map<Email>(c.Email), c.Password, new DelegatingSupplier<Guid>(Guid.NewGuid)));

        CreateMap<RegisterRequest, UserIdQuery>();
        CreateMap<UserIdResult, RegisterResponse>();

        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();
    }
}
