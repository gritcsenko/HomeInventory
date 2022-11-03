using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Authentication.Commands.Register;
public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : ICommand<RegistrationResult>;
