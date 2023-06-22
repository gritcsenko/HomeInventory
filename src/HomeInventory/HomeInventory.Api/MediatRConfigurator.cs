using HomeInventory.Application;
using MediatR;
using MediatR.NotificationPublishers;
using MediatR.Registration;

namespace HomeInventory.Api;

internal static class MediatRConfigurator
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var serviceConfig = new MediatRServiceConfiguration()
            .RegisterServicesFromAssemblies(Application.AssemblyReference.Assembly)
            .AddLoggingBehavior()
            .AddUnitOfWorkBehavior()
            .SetNotificationPublisher<TaskWhenAllPublisher>();

        ServiceRegistrar.AddMediatRClasses(services, serviceConfig);
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);

        return services;
    }

    public static MediatRServiceConfiguration SetNotificationPublisher<T>(this MediatRServiceConfiguration configuration)
        where T : INotificationPublisher
    {
        configuration.NotificationPublisherType = typeof(T);
        return configuration;
    }
}
