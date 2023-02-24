using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : Repository<UserModel, User>, IUserRepository
{
    public UserRepository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
        : base(contextFactory, mapper, evaluator, dateTimeService)
    {
    }

    public async Task<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<OneOf<User, NotFound>> FindFirstByEmailOrNotFoundUserAsync(Email email, CancellationToken cancellationToken = default) =>
        await FindFirstOrNotFoundAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default)
    {
        var userResult = await FindFirstOrNotFoundAsync(new UserHasIdSpecification(userId), cancellationToken);
        return userResult.Match(
            user => true,
            notFound => false);
    }
}
