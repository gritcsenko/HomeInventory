using HomeInventory.Domain.Primitives.Events;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public sealed record class AuthenticateResult(Domain.ValueObjects.UserId Id, string Token);
