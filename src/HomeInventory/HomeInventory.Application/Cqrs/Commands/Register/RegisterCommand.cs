using DotNext;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public record class RegisterCommand(
    Email Email,
    string Password,
    ISupplier<Guid> UserIdSupplier) : ICommand;
