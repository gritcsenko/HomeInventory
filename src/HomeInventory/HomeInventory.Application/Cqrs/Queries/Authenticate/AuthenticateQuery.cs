using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;
public record class AuthenticateQuery(
    string Email,
    string Password
    ) : IQuery<AuthenticateResult>;
