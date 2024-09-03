using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using MediatR.NotificationPublishers;
using MediatR.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrSupportModule : BaseAttachableModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var serviceConfig = new MediatRServiceConfiguration();
        serviceConfig.AddOpenBehavior(typeof(LoggingBehavior<,>));
        serviceConfig.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        serviceConfig.NotificationPublisherType = typeof(TaskWhenAllPublisher);

        foreach (var module in FindModules<IModuleWithMediatr>())
        {
            module.Configure(serviceConfig);
        }

        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
    }
}
