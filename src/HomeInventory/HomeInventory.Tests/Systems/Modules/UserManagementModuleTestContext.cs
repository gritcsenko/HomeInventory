using HomeInventory.Web.UserManagement;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class UserManagementModuleTestContext(BaseTest test) : BaseApiModuleGivenTestContext<UserManagementModuleTestContext, UserManagementCarterModule>(test)
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);
        services.AddScoped<ContractsMapper>();
    }
}
