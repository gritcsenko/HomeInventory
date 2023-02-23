namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

public record class AuthenticateResult(Domain.ValueObjects.UserId Id, string Token);
