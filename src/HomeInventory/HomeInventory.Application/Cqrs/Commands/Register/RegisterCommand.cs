using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;
public record class RegisterCommand(
    string FirstName,
    string LastName,
    Email Email,
    string Password) : ICommand<RegistrationResult>;
