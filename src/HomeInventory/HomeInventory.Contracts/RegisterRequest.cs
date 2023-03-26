namespace HomeInventory.Contracts;

public record class RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);
