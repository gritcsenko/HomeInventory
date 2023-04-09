using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration SetNotificationPublisher<T>(this MediatRServiceConfiguration configuration, T instance)
        where T : INotificationPublisher
    {
        configuration.NotificationPublisher = instance;
        return configuration;
    }

    public static MediatRServiceConfiguration SetNotificationPublisher<T>(this MediatRServiceConfiguration configuration, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where T : INotificationPublisher
    {
        configuration.NotificationPublisherType = typeof(T);
        configuration.Lifetime = lifetime;
        return configuration;
    }
}
