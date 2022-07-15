using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;

public record class AuthenticateResult(UserId Id, string Token);
