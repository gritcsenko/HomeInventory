﻿using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

internal sealed class UserIdQueryHandler(IUserRepository userRepository) : QueryHandler<UserIdQuery, UserIdResult>
{
    private readonly IUserRepository _repository = userRepository;

    protected override async Task<OneOf<UserIdResult, IError>> InternalHandle(UserIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.FindFirstByEmailUserOptionalAsync(query.Email, cancellationToken);
        return result
            .Convert<OneOf<UserIdResult, IError>>(user => new UserIdResult(user.Id))
            .OrInvoke(() => new NotFoundError($"User with email {query.Email} was not found"));
    }
}
