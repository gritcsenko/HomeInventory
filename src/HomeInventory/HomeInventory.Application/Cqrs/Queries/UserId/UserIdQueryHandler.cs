using FluentResults;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

internal class UserIdQueryHandler : QueryHandler<UserIdQuery, UserIdResult>
{
    private readonly IUserRepository _repository;

    public UserIdQueryHandler(IUserRepository userRepository)
    {
        _repository = userRepository;
    }

    protected override async Task<Result<UserIdResult>> InternalHandle(UserIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.FindFirstOrNotFoundAsync(UserSpecifications.HasEmail(query.Email), cancellationToken);
        if (result.TryPickT1(out _, out var user))
        {
            return new NotFoundError($"User with email {query.Email} was not found");
        }

        return new UserIdResult(user.Id);
    }
}
