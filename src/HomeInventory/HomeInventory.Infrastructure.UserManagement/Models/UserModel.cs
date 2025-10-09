using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure.Framework.Models;

namespace HomeInventory.Infrastructure.UserManagement.Models;

internal sealed class UserModel : IPersistentModel<UserId>, IHasCreationAudit, IHasModificationAudit
{
    public required UserId Id { get; init; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset ModifiedOn { get; set; }
}
