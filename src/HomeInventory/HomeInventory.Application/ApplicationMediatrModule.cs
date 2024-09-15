using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Framework;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrModule : BaseModuleWithMediatr
{
    public override void Configure(MediatRServiceConfiguration configuration)
    {
        configuration.NotificationPublisherType = typeof(TaskWhenAllPublisher);
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        configuration.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));

        RegisterServicesFromCurrentAssembly(configuration);
    }
}
