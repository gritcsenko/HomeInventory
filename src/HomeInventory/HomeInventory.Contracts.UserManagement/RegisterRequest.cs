namespace HomeInventory.Contracts.UserManagement;

public record RegisterRequest(
    string Email,
    string Password);
