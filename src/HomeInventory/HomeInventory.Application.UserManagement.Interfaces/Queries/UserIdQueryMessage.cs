using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

public sealed record class UserIdQueryMessage(Cuid Id, DateTimeOffset CreatedOn, Email Email) : IRequestMessage<UserIdResult>;
