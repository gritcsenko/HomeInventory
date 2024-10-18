namespace HomeInventory.Contracts.UserManagement;

public record class RegisterRequest(
    string Email,
    string Password);
