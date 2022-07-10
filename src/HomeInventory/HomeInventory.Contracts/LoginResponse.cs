namespace HomeInventory.Contracts;

public record class LoginResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Token);