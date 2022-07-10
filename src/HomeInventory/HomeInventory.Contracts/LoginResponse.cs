namespace HomeInventory.Contracts;

public record class LoginResponse(
    string Id,
    string FirstName,
    string LastName,
    string Token);