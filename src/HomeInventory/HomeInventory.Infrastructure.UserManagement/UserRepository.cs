using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Infrastructure.Persistence;

internal sealed class UserRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator, IEventsPersistenceService eventsPersistenceService)
    : Repository<UserModel, User, UserId>(context, mapper, evaluator, eventsPersistenceService), IUserRepository
{
    public async Task<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<Option<User>> FindFirstByEmailUserOptionalAsync(Email email, CancellationToken cancellationToken = default) =>
        await FindFirstOptionAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default)
    {
        var userResult = await FindFirstOptionAsync(new ByIdFilterSpecification<UserModel, UserId>(userId), cancellationToken)
            .Convert(x => true);
#pragma warning disable CA1849 // Call async methods when in an async method
        return userResult.IfNone(false);
#pragma warning restore CA1849 // Call async methods when in an async method
    }
}
