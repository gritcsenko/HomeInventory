using Ardalis.Specification;
using AutoMapper;
using DotNext;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : Repository<UserModel, User>, IUserRepository
{
    public UserRepository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator)
        : base(contextFactory, mapper, evaluator)
    {
    }

    public ValueTask<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public ValueTask<Optional<User>> FindFirstByEmailUserOptionalAsync(Email email, CancellationToken cancellationToken = default) =>
        FindFirstOptionalAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async ValueTask<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default)
    {
        var userResult = await FindFirstOptionalAsync(new UserHasIdSpecification(userId), cancellationToken);
        return userResult.HasValue;
    }
}
