using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public sealed record class RegisterUserRequestMessage(Ulid Id, DateTimeOffset CreatedOn, Email Email, string Password) : IRequestMessage<Option<Error>>;
