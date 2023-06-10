namespace HomeInventory.Web.Authorization.Dynamic;

public class PermissionMetadata
{
    public PermissionMetadata(IReadOnlyCollection<PermissionType> permissions) => Permissions = permissions;

    public IReadOnlyCollection<PermissionType> Permissions { get; }
}
