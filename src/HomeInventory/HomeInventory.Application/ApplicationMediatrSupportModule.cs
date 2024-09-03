using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using MediatR.NotificationPublishers;
using MediatR.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrSupportModule : BaseModule, IAttachableModule
{
    private IReadOnlyCollection<IModule> _modules = [];

    public void OnAttached(IReadOnlyCollection<IModule> modules)
    {
        _modules = modules;
    }

    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var serviceConfig = new MediatRServiceConfiguration();
        serviceConfig.AddOpenBehavior(typeof(LoggingBehavior<,>));
        serviceConfig.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        serviceConfig.NotificationPublisherType = typeof(TaskWhenAllPublisher);

        foreach (var module in _modules.OfType<IModuleWithMediatr>())
        {
            module.Configure(serviceConfig);
        }

        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
    }
}
