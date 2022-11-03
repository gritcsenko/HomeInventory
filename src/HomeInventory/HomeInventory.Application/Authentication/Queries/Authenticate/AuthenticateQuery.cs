using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
public record class AuthenticateQuery(
    Email Email,
    string Password
    ) : IQuery<AuthenticateResult>;
