using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public sealed record class RegisterUserRequestMessage(Cuid Id, DateTimeOffset CreatedOn, Email Email, string Password) : IRequestMessage;
