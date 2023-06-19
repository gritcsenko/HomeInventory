using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Infrastructure.Persistence.Models;

internal class UserModel : IPersistentModel<UserId>, ICreationAuditableModel, IModificationAuditableModel
{
    public required UserId Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
}
