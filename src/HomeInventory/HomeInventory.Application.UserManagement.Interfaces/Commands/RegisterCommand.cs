using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Interfaces.Commands;

public sealed record class RegisterCommand(
    Email Email,
    string Password) : ICommand;
