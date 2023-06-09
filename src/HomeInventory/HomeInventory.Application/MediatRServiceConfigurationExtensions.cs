using HomeInventory.Application.Cqrs.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration SetNotificationPublisher<T>(this MediatRServiceConfiguration configuration)
        where T : INotificationPublisher
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.NotificationPublisherType = typeof(T);
        return configuration;
    }

    public static MediatRServiceConfiguration AddLoggingBehavior(this MediatRServiceConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
    }

    public static MediatRServiceConfiguration AddUnitOfWorkBehavior(this MediatRServiceConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return configuration.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
    }
}
