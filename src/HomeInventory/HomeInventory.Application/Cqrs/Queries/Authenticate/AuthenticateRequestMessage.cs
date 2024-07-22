using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public sealed record class AuthenticateRequestMessage(
    Ulid Id,
    DateTimeOffset CreatedOn,
    Email Email,
    string Password
    ) : IRequestMessage<AuthenticateResult>;
