﻿using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Framework.Mapping;
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
