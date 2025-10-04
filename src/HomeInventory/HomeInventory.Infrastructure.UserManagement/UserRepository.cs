using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Framework.Specifications;
using HomeInventory.Infrastructure.UserManagement.Models;
using HomeInventory.Infrastructure.UserManagement.Specifications;

namespace HomeInventory.Infrastructure.UserManagement;

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
            .Convert(static _ => true);
#pragma warning disable CA1849 // Call async methods when in an async method
        return userResult.IfNone(false);
#pragma warning restore CA1849 // Call async methods when in an async method
    }
}
