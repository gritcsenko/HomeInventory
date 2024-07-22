using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

public sealed record class UserIdQueryMessage(Ulid Id, DateTimeOffset CreatedOn, Email Email) : IRequestMessage<UserIdResult>;
