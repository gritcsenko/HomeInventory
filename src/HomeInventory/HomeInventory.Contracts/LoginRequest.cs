namespace HomeInventory.Contracts;

public record LoginRequest(
    string Email,
    string Password);
