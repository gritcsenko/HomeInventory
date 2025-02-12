using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public record AuthenticateResult(UserId Id, string Token);
