namespace HomeInventory.Web.Authorization.Dynamic;

public class PermissionMetadata(IReadOnlyCollection<PermissionType> permissions)
{
    public IReadOnlyCollection<PermissionType> Permissions { get; } = permissions;
}
