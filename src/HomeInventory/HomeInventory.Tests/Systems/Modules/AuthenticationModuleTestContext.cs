using HomeInventory.Web;
using HomeInventory.Web.Modules;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class AuthenticationModuleTestContext(BaseTest test) : BaseApiModuleGivenTestContext<AuthenticationModuleTestContext, AuthenticationModule>(test)
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);
        services.AddScoped<ContractsMapper>();
    }
}
