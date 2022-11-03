using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Authentication.Commands.Register;
public record RegisterCommand(
    string FirstName,
    string LastName,
    Email Email,
    string Password) : ICommand<RegistrationResult>;
