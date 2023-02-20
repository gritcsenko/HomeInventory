namespace HomeInventory.Contracts;

public record class LoginRequest(
    string Email,
    string Password);
