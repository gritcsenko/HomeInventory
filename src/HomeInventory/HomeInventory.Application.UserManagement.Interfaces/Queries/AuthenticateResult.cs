using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Interfaces.Queries;

public record AuthenticateResult(UserId Id, string Token);
