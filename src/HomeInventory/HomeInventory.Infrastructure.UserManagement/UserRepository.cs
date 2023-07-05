using Ardalis.Specification;
using AutoMapper;
using DotNext;
using HomeInventory.Core;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : Repository<UserModel, User, UserId>, IUserRepository
{
    public UserRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator, IEventsPersistenceService eventsPersistenceService)
        : base(context, mapper, evaluator, eventsPersistenceService)
    {
    }

    public ValueTask<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public ValueTask<Optional<User>> FindFirstByEmailUserOptionalAsync(Email email, CancellationToken cancellationToken = default) =>
        FindFirstOptionalAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async ValueTask<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default)
    {
        var userResult = await FindFirstOptionalAsync(new ByIdFilterSpecification<UserModel, UserId>(userId), cancellationToken)
            .Convert(x => true);
        return userResult.Or(false);
    }
}
