using DotNext;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public sealed record class RegisterCommand(
    Email Email,
    string Password) : ICommand;
