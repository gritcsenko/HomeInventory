using HomeInventory.Application.Cqrs.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration SetNotificationPublisher<T>(this MediatRServiceConfiguration configuration)
        where T : INotificationPublisher
    {
        configuration.NotificationPublisherType = typeof(T);
        return configuration;
    }

    public static MediatRServiceConfiguration AddLoggingBehavior(this MediatRServiceConfiguration configuration) =>
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

    public static MediatRServiceConfiguration AddUnitOfWorkBehavior(this MediatRServiceConfiguration configuration) =>
        configuration.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
}
