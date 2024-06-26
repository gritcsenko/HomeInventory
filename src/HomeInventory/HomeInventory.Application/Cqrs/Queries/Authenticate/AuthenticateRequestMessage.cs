using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public sealed record class AuthenticateRequestMessage(
    Cuid Id,
    DateTimeOffset CreatedOn,
    Email Email,
    string Password
    ) : IRequestMessage<AuthenticateResult>;
