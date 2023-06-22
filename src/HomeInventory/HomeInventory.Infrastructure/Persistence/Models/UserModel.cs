using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Infrastructure.Persistence.Models;

internal class UserModel : IPersistentModel<UserId>, IHasCreationAudit, IHasModificationAudit
{
    public required UserId Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset ModifiedOn { get; set; }
}
