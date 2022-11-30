using AutoMapper;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : BaseRepository<UserModel, User>, IUserRepository
{
    public UserRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
        : base(context, mapper, evaluator)
    {
    }

    public async Task<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<OneOf<User, NotFound>> FindFirstByEmailOrNotFoundUserAsync(Email email, CancellationToken cancellationToken = default) =>
        await FindFirstOrNotFoundAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<bool> HasPermissionAsync(UserId userId, string tag, CancellationToken cancellationToken = default)
    {
        var userResult = await FindFirstOrNotFoundAsync(new UserHasIdSpecification(userId), cancellationToken);
        return userResult.Match(
            user => true,
            notFound => false);
    }
}
