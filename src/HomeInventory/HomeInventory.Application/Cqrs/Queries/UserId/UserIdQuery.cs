using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

public record UserIdQuery(
    string Email) : IQuery<UserIdResult>;
