using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using MapsterMapper;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : BaseRepository<UserModel, User>, IUserRepository
{
    public UserRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
        : base(context, mapper, evaluator)
    {
    }

    public async Task<bool> IsUserHasEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await HasAsync(new UserHasEmailSpecification(email), cancellationToken);

    public async Task<OneOf<User, NotFound>> FindFirstByEmailOrNotFoundUserAsync(string email, CancellationToken cancellationToken = default) =>
        await FindFirstOrNotFoundAsync(new UserHasEmailSpecification(email), cancellationToken);
}
