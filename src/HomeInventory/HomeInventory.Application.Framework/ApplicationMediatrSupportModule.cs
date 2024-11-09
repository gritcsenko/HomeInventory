using HomeInventory.Modules.Interfaces;
using MediatR.NotificationPublishers;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public sealed class ApplicationMediatrSupportModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        var serviceConfig = new MediatRServiceConfiguration
        {
            NotificationPublisherType = typeof(TaskWhenAllPublisher)
        };

        foreach (var module in context.Modules.OfType<IModuleWithMediatr>())
        {
            module.Configure(serviceConfig);
            serviceConfig.RegisterServicesFromAssemblyContaining(module.GetType());
        }

        ServiceRegistrar.AddMediatRClasses(context.Services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(context.Services, serviceConfig);
    }
}
