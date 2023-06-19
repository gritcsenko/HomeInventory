namespace HomeInventory.Contracts;

public record class LoginResponse(
    Ulid Id,
    string Token);
