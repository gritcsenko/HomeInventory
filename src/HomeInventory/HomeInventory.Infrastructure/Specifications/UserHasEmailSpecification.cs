using Ardalis.Specification;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasEmailSpecification : Specification<UserModel>, ISingleResultSpecification<UserModel>, ICompiledSingleResultSpecification<UserModel>
{
    private static readonly Func<DatabaseContext, string, CancellationToken, Task<UserModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DatabaseContext ctx, string email, CancellationToken _) => ctx.Set<UserModel>().FirstOrDefault(x => x.Email == email));
    private readonly Email _email;

    public UserHasEmailSpecification(Email email)
    {
        Query.Where(x => x.Email == email.Value);
        _email = email;
    }

    public Task<UserModel?> ExecuteAsync(DatabaseContext context, CancellationToken cancellationToken) => _cachedQuery(context, _email.Value, cancellationToken);
}
