namespace HomeInventory.Contracts;

public record class LoginResponse(
    Guid Id,
    string Token);
