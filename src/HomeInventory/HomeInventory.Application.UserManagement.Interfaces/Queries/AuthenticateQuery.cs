using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Interfaces.Queries;

public record AuthenticateQuery(Email Email, string Password) : IQuery<AuthenticateResult>;
