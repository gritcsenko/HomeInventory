﻿using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasIdSpecification : ByIdFilterSpecification<UserModel>
{
    public UserHasIdSpecification(UserId id)
        : base(id.Id)
    {
    }
}
