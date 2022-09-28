using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
public record class AuthenticateQuery(
    string Email,
    string Password
    ) : IQuery<AuthenticateResult>;
