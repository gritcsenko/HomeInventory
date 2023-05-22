namespace HomeInventory.Contracts;

public record class RegisterRequest(
    string Email,
    string Password);
