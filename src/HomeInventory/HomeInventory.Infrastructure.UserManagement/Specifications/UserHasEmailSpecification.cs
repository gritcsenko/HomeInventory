﻿using Ardalis.Specification;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal sealed class UserHasEmailSpecification : Specification<UserModel>, ISingleResultSpecification<UserModel>, ICompiledSingleResultSpecification<UserModel>
{
    private static readonly Func<DbContext, string, CancellationToken, Task<UserModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, string email, CancellationToken _) => ctx.Set<UserModel>().FirstOrDefault(x => x.Email == email));
    private readonly Email _email;

    public UserHasEmailSpecification(Email email)
    {
        Query.Where(x => x.Email == email.Value);
        _email = email;
    }

    public Task<UserModel?> ExecuteAsync(DbContext context, CancellationToken cancellationToken) => _cachedQuery(context, _email.Value, cancellationToken);
}
