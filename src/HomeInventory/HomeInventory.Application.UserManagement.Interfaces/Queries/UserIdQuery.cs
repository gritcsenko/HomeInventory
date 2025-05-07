using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Interfaces.Queries;

public record UserIdQuery(
    Email Email) : IQuery<UserIdResult>;
