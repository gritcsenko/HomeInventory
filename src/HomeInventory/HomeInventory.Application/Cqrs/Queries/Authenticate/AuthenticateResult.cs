using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public record class AuthenticateResult(UserId Id, string Token);
