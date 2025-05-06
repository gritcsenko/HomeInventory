using HomeInventory.Tests.Systems.Modules;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Modules;

namespace HomeInventory.Tests.Presentation.Web;

public sealed class PermissionModuleTestsGivenContext(BaseTest test) : BaseApiModuleGivenTestContext<PermissionModuleTestsGivenContext, PermissionModule>(test)
{
    public PermissionModuleTestsGivenContext Permissions(out IVariable<IReadOnlyCollection<PermissionType>> permissionTypesVar) => 
        New(out permissionTypesVar, () => [.. CreateMany<PermissionType>(3)]);

    public PermissionModuleTestsGivenContext PermissionList(out IVariable<PermissionList> permissionListVar, IVariable<IReadOnlyCollection<PermissionType>> permissionTypesVar) => 
        New(out permissionListVar, permissionTypesVar, types => [.. types]);
}
