namespace HomeInventory.Web.Authorization.Dynamic;

public class PermissionMetadata
{
    public PermissionMetadata(IReadOnlyCollection<Permission> permissions) => Permissions = permissions;

    public IReadOnlyCollection<Permission> Permissions { get; }
}
