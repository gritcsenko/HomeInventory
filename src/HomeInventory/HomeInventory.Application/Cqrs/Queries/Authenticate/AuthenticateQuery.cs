using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public record class AuthenticateQuery(
    Email Email,
    string Password
    ) : IQuery<AuthenticateResult>;
