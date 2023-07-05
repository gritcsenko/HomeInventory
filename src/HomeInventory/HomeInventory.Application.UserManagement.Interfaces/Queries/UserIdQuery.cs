using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

public record UserIdQuery(
    Email Email) : IQuery<UserIdResult>;
