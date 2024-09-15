using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using MediatR.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrSupportModule : BaseAttachableModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var serviceConfig = new MediatRServiceConfiguration();

        foreach (var module in FindModules<IModuleWithMediatr>())
        {
            module.Configure(serviceConfig);
        }

        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
    }
}
