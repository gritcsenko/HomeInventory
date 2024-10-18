using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class ApplicationFeaturesModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFeatureManagement();
    }
}
